using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WandManager : MonoBehaviour
{
    public Quaternion vec;
    public bool debug_prints;


    static WandManager instance;

    public static WandManager Instance
    {
        get { return instance; }
    }

    public enum Button : int
    {
        LEFT = 0,
        RIGHT = 1
    };

    private bool[] buttons_down = new bool[2];
    private bool[] buttons_up = new bool[2];
    private bool[] buttons = new bool[2];
    private bool[] buttons_ = new bool[2];

    private IntPtr handle;
    private const int rawhidRxSize = 20;
    private const int rawhidTxSize = 20;
    private byte[] rec_buffer;
    private byte[] dummy_buf;
    private byte[] send_buffer_;
    private byte[] setup_buffer;
    private int missed_packets = 0;
    private float time_since_last_attempt = 0;
    private bool connected = false;
    private bool do_dequeue = false;

    private const byte QUAT_START = 3;

    private enum Command : byte
    {
        SETUP = 0x69,
        NULL = 0x1,
        RUMBLE = 0x2,
    };

    private const int USB_VID = 0x2341;
    private const int USB_PID = 0x8036;
    private float UNITY_TICKRATE_SECS = 0.03f;
    private const byte RECV_TICKRATE_MS = 20;
    private const byte WAND_TICKRATE_MS = 0;
    private const byte RADIO_TIMEOUT = 5;
    private const byte HID_TIMEOUT = 20;
    private const int HID_TIMEOUT_UNITY = 20;
    private const float HID_IDLE_TIMEOUT = 1.5f;
    private Queue<byte[]> sendQueue = new Queue<byte[]>();
    private byte hid_timestamp;
    private byte prev_timestamp;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);
            instance = this;
        }

    IEnumerator TransmitLoop(){

        for(;;)
        {
             if (connected)
            {
                    lock (sendQueue)
                    {
                        if (sendQueue.Count > 0)
                        {
                            hid_write(sendQueue.Peek());
                        }
                        else
                        {
                            // write nothing
                            hid_write(dummy_buf);
                        }
                    }
                    if (hid_read_confirmed()){
                        lock (sendQueue)
                        {
                            if (do_dequeue)
                            {
                                sendQueue.Dequeue();
                                do_dequeue = false;
                            }
                        }
                    }
            }
            else
            {
                if (time_since_last_attempt <= 0)
                {
                    DebugPrint("Attempting to connect to USB receiver...");
                    connected = TryConnect();
                    time_since_last_attempt = 2f;
                }
                else
                {
                    time_since_last_attempt -= UNITY_TICKRATE_SECS;
                }

            }
            yield return new WaitForSeconds(UNITY_TICKRATE_SECS);
        }
    }

   
    void hid_write(byte[] buf)
    {
        buf[0] = hid_timestamp;
        for (int i = 0; i < rawhidTxSize; ++i)
        {
            send_buffer_[i + 1] = buf[i];
        }
        HIDapi.hid_write(handle, send_buffer_, new UIntPtr(rawhidTxSize + 1));
    }

    bool hid_read_confirmed()
    {
        if (hid_read() < 1)
        {
            ++missed_packets;
            DebugPrintError("uh oh");
            if (missed_packets > 10)
            {
                DebugPrint("USB receiver disconnected.");
                connected = false;
            }
            return false;
        }
        return true;
        //else if (rec_buffer[0] == hid_timestamp)
        //{
        //    return true;
        //}
        //DebugPrintError(rec_buffer[0]);
        //DebugPrintError(hid_timestamp);
        //return false;
    }

    int hid_read()
    {
        int res;
        byte[] r = new byte[rawhidRxSize];
        res = HIDapi.hid_read_timeout(handle, r, new UIntPtr(rawhidRxSize), HID_TIMEOUT_UNITY);
        if (res > 0)
        {
            lock (rec_buffer)
            {
                rec_buffer = r;
            }
        }
        return res;
    }

    bool TryConnect()
    {
        IntPtr ptr = HIDapi.hid_enumerate(USB_VID, USB_PID);
        if (ptr == IntPtr.Zero)
        {
            HIDapi.hid_free_enumeration(ptr);
            DebugPrint("USB receiver not found.");
            return false;
        }
        hid_device_info enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));
        handle = HIDapi.hid_open_path(enumerate.path);
        HIDapi.hid_set_nonblocking(handle, 1);
        HIDapi.hid_free_enumeration(ptr);
        addReport(setup_buffer);
        return true;
    }

    void Start()
    {
        dummy_buf = new byte[rawhidTxSize];
        for (int i = 0; i < rawhidTxSize; ++i)
        {
            dummy_buf[i] = 0;
        }
        dummy_buf[1] = (byte)Command.NULL;
        hid_timestamp = 2;
        setup_buffer = new byte[rawhidTxSize];
        setup_buffer[1] = (byte)Command.SETUP;
        setup_buffer[2] = RECV_TICKRATE_MS;
        setup_buffer[3] = RADIO_TIMEOUT;
        setup_buffer[4] = HID_TIMEOUT;
        setup_buffer[5] = WAND_TICKRATE_MS;
        rec_buffer = new byte[rawhidRxSize];
        HIDapi.hid_init();
        connected = TryConnect();
        send_buffer_ = new byte[rawhidTxSize + 1];
        send_buffer_[0] = 0;
        StartCoroutine("TransmitLoop");
    }

    void addReport(byte[] buf)
    {
        lock (sendQueue)
        {
            sendQueue.Enqueue(buf);
        }
        do_dequeue = true;
        ++hid_timestamp;
    }
    // Update is called once per frame
    void Update()
    {
        Int16[] quat_raw = { 0, 0, 0, 0 };
        for (int i = 0; i < 4; ++i)
        {
            quat_raw[i] = (Int16)((rec_buffer[QUAT_START + i * 2] << 8) | rec_buffer[1 + QUAT_START + i * 2]);
            quat_raw[i] = (Int16)((rec_buffer[QUAT_START + i * 2] << 8) | rec_buffer[1 + QUAT_START + i * 2]);
        }
        vec.w = quat_raw[0] * 1f / (1 << 14);
        vec.x = quat_raw[1] * 1f / (1 << 14);
        vec.y = quat_raw[2] * 1f / (1 << 14);
        vec.z = quat_raw[3] * 1f / (1 << 14);

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons_[i] = buttons[i];
        }

        buttons[0] = (rec_buffer[1] & 0x02) != 0;
        buttons[1] = (rec_buffer[1] & 0x01) != 0;

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons_up[i] = (buttons_[i] & !buttons[i]);
            buttons_down[i] = (!buttons_[i] & buttons[i]);
        }
    }

    private void DebugPrintError(object s)
    {
        Debug.LogError(s);
    }

    private void DebugPrint(object s)
    {
        Debug.Log(s);
    }

    public void sendHapticEffect(int i)
    {
        byte[] rumble_buf = new byte[rawhidTxSize];
        rumble_buf[1] = (byte)Command.RUMBLE;
        rumble_buf[2] = 3;
        rumble_buf[3] = (byte)i;
        rumble_buf[4] = 0;
        addReport(rumble_buf);
    }

    public Quaternion GetQuat()
    {
        Quaternion vec2;
        vec2.x = vec.x;
        vec2.y = -vec.z;
        vec2.z = vec.y;

        vec2.w = vec.w;
        return vec2;
    }

    public bool getButtonDown(Button b)
    {
        return buttons_down[(int)b];
    }

    public bool getButton(Button b)
    {
        return buttons[(int)b];
    }
    
    public bool getButtonUp(Button b)
    {
        return buttons_up[(int)b];
    }
    
    void OnApplicationQuit(){
        connected = false;
        DebugPrint("Done");
        HIDapi.hid_close(handle);
    }
}
