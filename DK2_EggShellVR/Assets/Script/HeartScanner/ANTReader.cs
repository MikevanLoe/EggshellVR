using ANT_Managed_Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace ANT__Heartrate_Scanner
{
	public class HeartrateReciever
	{
		static readonly byte USER_ANT_CHANNEL = 0;         // ANT Channel to use
		static readonly ushort USER_DEVICENUM = 0;        // Device number    
		static readonly byte USER_DEVICETYPE = 120;          // Device type
		static readonly byte USER_TRANSTYPE = 0;           // Transmission type
		
		static readonly byte USER_RADIOFREQ = 57;          // RF Frequency + 2400 MHz
		static readonly ushort USER_CHANNELPERIOD = 8070;  // Channel Period
		
		static readonly byte[] USER_NETWORK_KEY = { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 };
		static readonly byte USER_NETWORK_NUM = 0;         // The network key is assigned to this network (channel) number
		
		static ANT_Device device0;
		static ANT_Channel channel0;
		static ANT_ReferenceLibrary.ChannelType channelType;
		static byte[] txBuffer = { 0, 0, 0, 0, 0, 0, 0, 0 };
		static bool bDisplay;
		static bool bBroadcasting;
		static int iIndex = 0;
		
		public static int Heartrate;
		
		static void Init()
		{
			try
			{
				Console.WriteLine("Attempting to connect to an ANT USB device...");
				device0 = new ANT_Device();   // Create a device instance using the automatic constructor (automatic detection of USB device number and baud rate)
				device0.deviceResponse += new ANT_Device.dDeviceResponseHandler(DeviceResponse);    // Add device response function to receive protocol event messages
				
				channel0 = device0.getChannel(USER_ANT_CHANNEL);    // Get channel from ANT device
				channel0.channelResponse += new dChannelResponseHandler(ChannelResponse);  // Add channel response function to receive channel event messages
				Console.WriteLine("Initialization was successful!");
			}
			catch (Exception ex)
			{
				//This part is likely to crash, but we want to catch that somewhere else
				//We keep the try catch anyway as a reminder.
				throw ex;
			}
		}
		
		
		////////////////////////////////////////////////////////////////////////////////
		// Start
		//
		// Start the ANT+ slave.
		// 
		// ucChannelType_:  ANT Channel Type. 0 = Master, 1 = Slave
		//                  If not specified, 2 is passed in as invalid.
		////////////////////////////////////////////////////////////////////////////////
		public static void Start(bool display = false)
		{
			Init();
			bDisplay = display;
			bBroadcasting = false;
			
			channelType = ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00;
			
			try
			{
				ConfigureANT();
				
				return;
			}
			catch (Exception ex)
			{
				throw new Exception("ANT+ setup failed: " + ex.Message + Environment.NewLine);
			}
		}
		
		/// <summary>
		/// Stops the ANT device from listening
		/// </summary>
		public static void Stop()
		{
			// Clean up ANT
			Console.WriteLine("Disconnecting module...");
			ANT_Device.shutdownDeviceInstance(ref device0);  // Close down the device completely and completely shut down all communication
			Console.WriteLine("Demo has completed successfully!");
		}
		
		////////////////////////////////////////////////////////////////////////////////
		// ConfigureANT
		//
		// Resets the system, configures the ANT channel and starts the demo
		////////////////////////////////////////////////////////////////////////////////
		private static void ConfigureANT()
		{
			Console.WriteLine("Resetting module...");
			device0.ResetSystem();     // Soft reset
			System.Threading.Thread.Sleep(500);    // Delay 500ms after a reset
			
			// If you call the setup functions specifying a wait time, you can check the return value for success or failure of the command
			// This function is blocking - the thread will be blocked while waiting for a response.
			// 500ms is usually a safe value to ensure you wait long enough for any response
			// If you do not specify a wait time, the command is simply sent, and you have to monitor the protocol events for the response,
			if (!device0.setNetworkKey(USER_NETWORK_NUM, USER_NETWORK_KEY, 500))
				throw new Exception("Error configuring network key");
			
			if (!channel0.assignChannel(channelType, USER_NETWORK_NUM, 500))
				throw new Exception("Error assigning channel");
			
			if (!channel0.setChannelID(USER_DEVICENUM, false, USER_DEVICETYPE, USER_TRANSTYPE, 500))  // Not using pairing bit
				throw new Exception("Error configuring Channel ID");
			
			if (!channel0.setChannelFreq(USER_RADIOFREQ, 500))
				throw new Exception("Error configuring Radio Frequency");
			
			if (!channel0.setChannelPeriod(USER_CHANNELPERIOD, 500))
				throw new Exception("Error configuring Channel Period");
			
			bBroadcasting = true;
			if (!channel0.openChannel(500))
			{
				bBroadcasting = false;
				throw new Exception("Error opening channel");
			}
			
			#if (ENABLE_EXTENDED_MESSAGES)
			// Extended messages are not supported in all ANT devices, so
			// we will not wait for the response here, and instead will monitor 
			// the protocol events
			Console.WriteLine("Enabling extended messages...");
			device0.enableRxExtendedMessages(true);
			#endif
		}
		
		////////////////////////////////////////////////////////////////////////////////
		// ChannelResponse
		//
		// Called whenever a channel event is recieved. 
		// 
		// response: ANT message
		////////////////////////////////////////////////////////////////////////////////
		static void ChannelResponse(ANT_Response response)
		{
			try
			{
				switch ((ANT_ReferenceLibrary.ANTMessageID)response.responseID)
				{
				case ANT_ReferenceLibrary.ANTMessageID.RESPONSE_EVENT_0x40:
				{
					switch (response.getChannelEventCode())
					{
						// This event indicates that a message has just been
						// sent over the air. We take advantage of this event to set
						// up the data for the next message period.   
					case ANT_ReferenceLibrary.ANTEventID.EVENT_TX_0x03:
					{
						txBuffer[0]++;  // Increment the first byte of the buffer
						
						// Broadcast data will be sent over the air on
						// the next message period
						if (bBroadcasting)
						{
							channel0.sendBroadcastData(txBuffer);
							
							if (bDisplay)
							{
								// Echo what the data will be over the air on the next message period
								Console.WriteLine("Tx: (" + response.antChannel.ToString() + ")" + BitConverter.ToString(txBuffer));
							}
						}
						else
						{
							string[] ac = { "|", "/", "_", "\\" };
							Console.Write("Tx: " + ac[iIndex++] + "\r");
							iIndex &= 3;
						}
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_RX_SEARCH_TIMEOUT_0x01:
					{
						Console.WriteLine("Search Timeout");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_RX_FAIL_0x02:
					{
						//Console.WriteLine("Rx Fail");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_TRANSFER_RX_FAILED_0x04:
					{
						Console.WriteLine("Burst receive has failed");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_TRANSFER_TX_COMPLETED_0x05:
					{
						Console.WriteLine("Transfer Completed");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_TRANSFER_TX_FAILED_0x06:
					{
						Console.WriteLine("Transfer Failed");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_CHANNEL_CLOSED_0x07:
					{
						// This event should be used to determine that the channel is closed.
						Console.WriteLine("Channel Closed");
						Console.WriteLine("Unassigning Channel...");
						if (channel0.unassignChannel(500))
						{
							Console.WriteLine("Unassigned Channel");
							Console.WriteLine("Press enter to exit");
						}
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_RX_FAIL_GO_TO_SEARCH_0x08:
					{
						Console.WriteLine("Go to Search");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_CHANNEL_COLLISION_0x09:
					{
						Console.WriteLine("Channel Collision");
						break;
					}
					case ANT_ReferenceLibrary.ANTEventID.EVENT_TRANSFER_TX_START_0x0A:
					{
						Console.WriteLine("Burst Started");
						break;
					}
					default:
					{
						Console.WriteLine("Unhandled Channel Event " + response.getChannelEventCode());
						break;
					}
					}
					break;
				}
				case ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E:
				case ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
				case ANT_ReferenceLibrary.ANTMessageID.BURST_DATA_0x50:
				case ANT_ReferenceLibrary.ANTMessageID.EXT_BROADCAST_DATA_0x5D:
				case ANT_ReferenceLibrary.ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E:
				case ANT_ReferenceLibrary.ANTMessageID.EXT_BURST_DATA_0x5F:
				{
					if (bDisplay)
					{
						if (response.isExtended()) // Check if we are dealing with an extended message
						{
							ANT_ChannelID chID = response.getDeviceIDfromExt();    // Channel ID of the device we just received a message from
							Console.Write("Chan ID(" + chID.deviceNumber.ToString() + "," + chID.deviceTypeID.ToString() + "," + chID.transmissionTypeID.ToString() + ") - ");
						}
						if (response.responseID == (byte)ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E
						    || response.responseID == (byte)ANT_ReferenceLibrary.ANTMessageID.EXT_BROADCAST_DATA_0x5D)
							Console.Write("Rx:(" + response.antChannel.ToString() + "): ");
						else if (response.responseID == (byte)ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F
						         || response.responseID == (byte)ANT_ReferenceLibrary.ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E)
							Console.Write("Acked Rx:(" + response.antChannel.ToString() + "): ");
						else
							Console.Write("Burst(" + response.getBurstSequenceNumber().ToString("X2") + ") Rx:(" + response.antChannel.ToString() + "): ");
						
						
					}
					byte[] buffer = response.getDataPayload();
					HeartrateReciever.Heartrate = (int)buffer[7]; //Assign heartrate to field
					break;
				}
				default:
				{
					Console.WriteLine("Unknown Message " + response.responseID);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Channel response processing failed with exception: " + ex.Message);
			}
		}
		
		
		
		////////////////////////////////////////////////////////////////////////////////
		// DeviceResponse
		//
		// Called whenever a message is received from ANT unless that message is a 
		// channel event message. 
		// 
		// response: ANT message
		////////////////////////////////////////////////////////////////////////////////
		static void DeviceResponse(ANT_Response response)
		{
			switch ((ANT_ReferenceLibrary.ANTMessageID)response.responseID)
			{
			case ANT_ReferenceLibrary.ANTMessageID.STARTUP_MESG_0x6F:
			{
				Console.Write("RESET Complete, reason: ");
				
				byte ucReason = response.messageContents[0];
				
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_POR_0x00)
					Console.WriteLine("RESET_POR");
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_RST_0x01)
					Console.WriteLine("RESET_RST");
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_WDT_0x02)
					Console.WriteLine("RESET_WDT");
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_CMD_0x20)
					Console.WriteLine("RESET_CMD");
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_SYNC_0x40)
					Console.WriteLine("RESET_SYNC");
				if (ucReason == (byte)ANT_ReferenceLibrary.StartupMessage.RESET_SUSPEND_0x80)
					Console.WriteLine("RESET_SUSPEND");
				break;
			}
			case ANT_ReferenceLibrary.ANTMessageID.VERSION_0x3E:
			{
				Console.WriteLine("VERSION: " + new ASCIIEncoding().GetString(response.messageContents));
				break;
			}
			case ANT_ReferenceLibrary.ANTMessageID.RESPONSE_EVENT_0x40:
			{
				switch (response.getMessageID())
				{
				case ANT_ReferenceLibrary.ANTMessageID.CLOSE_CHANNEL_0x4C:
				{
					if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.CHANNEL_IN_WRONG_STATE_0x15)
					{
						Console.WriteLine("Channel is already closed");
						Console.WriteLine("Unassigning Channel...");
						if (channel0.unassignChannel(500))
						{
							Console.WriteLine("Unassigned Channel");
							Console.WriteLine("Press enter to exit");
						}
					}
					break;
				}
				case ANT_ReferenceLibrary.ANTMessageID.NETWORK_KEY_0x46:
				case ANT_ReferenceLibrary.ANTMessageID.ASSIGN_CHANNEL_0x42:
				case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51:
				case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_RADIO_FREQ_0x45:
				case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_MESG_PERIOD_0x43:
				case ANT_ReferenceLibrary.ANTMessageID.OPEN_CHANNEL_0x4B:
				case ANT_ReferenceLibrary.ANTMessageID.UNASSIGN_CHANNEL_0x41:
				{
					if (response.getChannelEventCode() != ANT_ReferenceLibrary.ANTEventID.RESPONSE_NO_ERROR_0x00)
					{
						Console.WriteLine(String.Format("Error {0} configuring {1}", response.getChannelEventCode(), response.getMessageID()));
					}
					break;
				}
				case ANT_ReferenceLibrary.ANTMessageID.RX_EXT_MESGS_ENABLE_0x66:
				{
					if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.INVALID_MESSAGE_0x28)
					{
						Console.WriteLine("Extended messages not supported in this ANT product");
						break;
					}
					else if (response.getChannelEventCode() != ANT_ReferenceLibrary.ANTEventID.RESPONSE_NO_ERROR_0x00)
					{
						Console.WriteLine(String.Format("Error {0} configuring {1}", response.getChannelEventCode(), response.getMessageID()));
						break;
					}
					Console.WriteLine("Extended messages enabled");
					break;
				}
				case ANT_ReferenceLibrary.ANTMessageID.REQUEST_0x4D:
				{
					if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.INVALID_MESSAGE_0x28)
					{
						Console.WriteLine("Requested message not supported in this ANT product");
						break;
					}
					break;
				}
				default:
				{
					Console.WriteLine("Unhandled response " + response.getChannelEventCode() + " to message " + response.getMessageID()); break;
				}
				}
				break;
			}
			}
		}
	}
}
