using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class mqttTest : MonoBehaviour {
	private string brokerHost = "m10.cloudmqtt.com";
	private string username = "unity-astroids";
	private string password = "unity-09876";
	private string clientId;
	private string topicPrefix = "gigascapes";
	private int brokerPort = 12173;

	private MqttClient client;
	// Use this for initialization
	void Start () {
		clientId = Guid.NewGuid().ToString();

		// create client instance
		client = new MqttClient(brokerHost, brokerPort, false , null); 

		// register for disconnected event 
		client.MqttMsgDisconnected += client_MqttMsgDisconnected; 
		
		// register for message received event 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 

		client.Connect(clientId, username, password); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { AllPositionsTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	}
	public string AllPositionsTopic
	{
		get { return topicPrefix + "/+/positions-ts"; }
	}
	public string SelfPositionsTopic
	{
		get { return topicPrefix + "/" + clientId + "/positions"; }
	}

	void client_MqttMsgDisconnected(object self, System.EventArgs e) 
	{ 
		Debug.Log("Disconnected");
	} 
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 

		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
	} 

	void OnGUI(){
		if ( GUI.Button (new Rect (20,40,80,20), "Level 1")) {
			string payload = "{\"positions\":[{\"x\":0.49822380106572,\"y\":0.23090586145648}]}";
			Debug.Log("sending on topic:" + SelfPositionsTopic);
			Debug.Log("payload:" + payload);
			client.Publish(SelfPositionsTopic, 
				System.Text.Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			Debug.Log("sent");
		}
	}
	// Update is called once per frame
	void Update () {

	}
}
