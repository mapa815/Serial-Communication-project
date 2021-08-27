﻿/*
 * TP Project part 2
 * Created:     May 2020
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;  //SerialPort class
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

struct Telemetry
{
    public char[] charhandler;
};

namespace telecommand
{
    public partial class Form1 : Form
    {
        //array saving data length of telemetry packet
        String[] description = {"Satellite Time", "Axis Angles", "Primary Bus Voltage", "Battery Voltage", "Matrix Current",
        "Load Current","Charge Current","Discharge Current"};
        String[] tcmname = {"OBDH Telecontrol test","OBDH Indirect reset","OBDH A set to B indirectly",
        "OBDH B switch to A indirectly","OBDH enter safety mode","OBDH enter normal mode","Space Camera real-time photo","RS Camera real-time photo",
        "RS camera delay photo","Download real-time photo of space camera","Download delay photo of RS camera","Download real-time photo of RS camera",
        "ADS-B ON","ADS-B delay ON","Download delay data of ADS-B","Set satellite time","Set delay telemetry","Download delay telemetry data","Deploy VU antenna",
        "Store information of delay telemetry"};
        int[] lengths = { 7, 7, 7, 7, 7, 7, 7, 7, 13, 13, 13, 13, 8, 8, 8, 13, 6, 8, 7, 7};


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            updatePorts();           // updates port names
            CheckForIllegalCrossThreadCalls = false;
        }
        private void updatePorts()
        {
            // Refreshes the list of all COM ports
            string[] ports = SerialPort.GetPortNames();
            cmbPortName.Items.Clear();
            foreach (string port in ports)
            {
                cmbPortName.Items.Add(port);
            }
        }
        private SerialPort ComPort = new SerialPort();  //Initialise ComPort Variable as SerialPort
        private void connect()
        {
            bool error = false;

            // Check if all settings have been selected in the "Select port name" tab

            if (cmbPortName.SelectedIndex != -1 & cmbBaudRate.SelectedIndex != -1 & cmbParity.SelectedIndex != -1 & cmbDataBits.SelectedIndex != -1 & cmbStopBits.SelectedIndex != -1)
            {
                ComPort.PortName = cmbPortName.Text;
                ComPort.BaudRate = int.Parse(cmbBaudRate.Text);      //convert Text to Integer
                ComPort.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text); //convert Text to Parity
                ComPort.DataBits = int.Parse(cmbDataBits.Text);        //convert Text to Integer
                ComPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text);  //convert Text to stop bits

                try  // method to display a message instead of freezing in case of any error
                {
                    ComPort.Open(); //Open Port
                    ComPort.DataReceived += SerialPortDataReceived;  //Check for received data
                }
                catch (UnauthorizedAccessException) { error = true; }
                catch (System.IO.IOException) { error = true; }
                catch (ArgumentException) { error = true; }

                if (error) MessageBox.Show(this, "Could not open the COM port. Most likely it is already in use, has been removed, or is unavailable.", "COM Port unavailable", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            else
            {
                MessageBox.Show("Please select all the COM Serial Port Settings", "Serial Port Interface", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            //if the port is open, Change the Connect button to disconnect, enable the send button.
            //and disable the groupBox to prevent changing configuration of an open port.
            if (ComPort.IsOpen)
            {
                btnConnect.Text = "Disconnect";
                btnSend.Enabled = true;
                if (!rdText.Checked & !rdHex.Checked)  //if no data mode is selected, then select text mode by default
                {
                    rdHex.Checked = true;
                }
                groupBox1.Enabled = false;


            }
        }
        // Call this function to close the port.
        private void disconnect()
        {
            ComPort.Close();
            btnConnect.Text = "Connect";
            btnSend.Enabled = false;
            groupBox1.Enabled = true;

        }
        //whenever the connect button is clicked, it will check if the port is already open, call the disconnect function.
        // if the port is closed, call the connect function.
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (ComPort.IsOpen)
            {
                disconnect();
            }
            else
            {
                connect();
            }
        }

        // aux function to get strings from the char array
        private String charToString(char[] char_arr)
        {
            String str = new string(char_arr);
            return str;
        }

        // Function to send data to the serial port
        private void sendData()
        {
            bool error = false;
            if (rdText.Checked == true)        //if text mode is selected, send data as tex
            {

               //test lines for telemetry ********

                //for (int i = 0; i < lengths.Length; i++)
                //{
                //    String outtext;
                //    try
                //    {
                //        outtext = txtSend.Text.Substring(counter, lengths[i]);
                //    }
                //    catch (ArgumentOutOfRangeException)
                //    {
                //        outtext = "00";
                //    }
                //    byte[] bytesvar = Encoding.Unicode.GetBytes(outtext);
                //    usingthis.charhandler = Encoding.Unicode.GetChars(bytesvar);
                //    String converting = charToString(usingthis.charhandler);
                //    // Show in the terminal window
                //    rtxtDataArea.ForeColor = Color.Green;    //write sent text data in green colour
                //    rtxtDataArea.AppendText(description[i] + "\n");
                //    rtxtDataArea.AppendText(converting.ToUpper() + "         " + DateTime.Now.ToString("hh:mm:ss.fff tt") + "\n");
                //    counter += lengths[i];

                //}
                    txtSend.Clear();                       //clear screen after sending data

            }
            else                    //if Hex mode is selected, send data in hexadecimal
            {
                if(txtSend.Text.Length == 2)
                {
                    try
                    {
                        int value = Convert.ToInt32(txtSend.Text, 16);

                        var auxout = txtSend.Text.PadRight(8, '0');
                        var minus1 = Regex.Replace(auxout, ".{2}", "$0 ");
                        var newoutput = minus1.Remove(minus1.Length - 1, 1);

                        Telemetry usingthis;
                        int counter = 0;
                        if (value >= 7 && value <= 18)
                        {
                            special(value);
                        }
                        else
                        {
                            rtxtDataArea.ForeColor = Color.Green;
                            rtxtDataArea.AppendText("\n" + newoutput + "           " + DateTime.Now.ToString("hh:mm:ss.fff tt") + "\n");
                            try
                            {
                                rtxtDataArea.AppendText(tcmname[value - 1] + "\n");
                            }
                            catch (IndexOutOfRangeException)
                            {
                                String errormessage = "Telecommand code not valid";
                                MessageBox.Show(errormessage);
                            }
                            //send data in bytes
                            //to test the telemetry this part can be edited to auxout variable
                            byte[] data = HexStringToByteArray(newoutput);
                            ComPort.Write(data, 0, data.Length);
                        }
                        txtSend.Clear();                       //clear screen after sending data
                    }
                    catch (FormatException) { error = true; }
                    // Inform the user if the hex string was not properly formatted
                    catch (ArgumentException) { error = true; }
                }
                else
                {
                    MessageBox.Show(this, "Not properly formatted hex string: " + txtSend.Text + "\n" + "Example: 01" + "\n" + "Range from 01 to 14 hex", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        //class to deal with special cases
        public void special(int num)
        {
            //use input box
            //cases
            string input = Interaction.InputBox("Input the following data to complete the telecontrol code"+"\n"+"Number (0x0000 to 0xFFFF)  Hour (0x00 to 0x17)  Minute (0x00 to 0x3B)  Second (0x00 to 0x3B)" + "\n" +"Input format example: 1234 12 12 12"
                , "Telecontrol Data Input", "0000 00 00 00");
        }

        //Convert a string of hex digits (example: E1 FF 1B) to a byte array. 
        //The string containing the hex digits (with or without spaces)
        //Returns an array of bytes. </returns>
        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        //This event will be raised when the form is closing.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ComPort.IsOpen) ComPort.Close();  //close the port if open when exiting the application.
        }
        //Data recived from the serial port is coming from another thread context than the UI thread.
        //Instead of reading the content directly in the SerialPortDataReceived, we need to use a delegate.
        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            //invokeRequired required compares the thread ID of the calling thread to the thread of the creating thread.
            // if these threads are different, it returns true
            if (this.rtxtDataArea.InvokeRequired)
            {
                rtxtDataArea.ForeColor = Color.Green;    //write text data in Green colour

                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.rtxtDataArea.AppendText(text);
            }
        }
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var data = serialPort.ReadExisting();
            //SetText(data);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //Clear the screen
            rtxtDataArea.Clear();
            txtSend.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            sendData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            updatePorts();           //Call this function to update port names
        }
    }
}
