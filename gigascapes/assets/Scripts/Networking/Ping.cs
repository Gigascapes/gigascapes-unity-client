using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking;
using uPLibrary;
using System;



public class Ping : MonoBehaviour {


	MqttClient client = new MqttClient("gigascapes-msg-broker.herokuapp.com");

	private void Start()
	{
		byte code = client.Connect(Guid.NewGuid().ToString());

		client.MqttMsgPublished += Client_MqttMsgPublished;


		ushort msgId = client.Publish("/my_topic",
									  new byte[] {00000000});

	}

	void Client_MqttMsgPublished(System.Object Sender, EventArgs e)
	{
		Debug.Log("Published " + "Message = " + e.ToString());
	}

}
