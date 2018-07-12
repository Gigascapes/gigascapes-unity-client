using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class NetworkManager : MonoBehaviour
{
    private string brokerHost = "m10.cloudmqtt.com";
    private string username = "unity-astroids";
    private string password = "unity-09876";
    private string clientId;
    private string otherClientId;
    private string topicPrefix = "gigascapes";
    private int brokerPort = 12173;
    private bool joined; //flip when connected to another clientID
    private readonly float sendFrequency = 0.1f; //seconds (this is also the update player2 frequency)

    public GameObject player1;
    public GameObject player2;//dummy network player2 in same space as player1

    private MqttClient client;
    private MWrapper myMessages;
    
    /*private readonly int numberOfMessagesToStore = 10;//change as needed, more cost performance and memory, 3 may be enough
    //array of messages, indexed by id ( linear search to find of size numberOfMessagesToStore)
    private Dictionary<int, MqttMessage> messageDictionary;
    private int[] messageHistory;
    private int messageHistoryPosition = 0;*/
    private int messageId = 0;  //message count


    private long myJoinTime;
    private long theirJoinTime;
    // Use this for initialization
    void Start()
    {
        joined = false;
        myJoinTime = 0;
        theirJoinTime = 0;
        clientId = Guid.NewGuid().ToString();
        otherClientId = null;
        // create client instance
        client = new MqttClient(brokerHost, brokerPort, false, null);

        // register for disconnected event 
        client.MqttMsgDisconnected += client_MqttMsgDisconnected;

        // register for message received event 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        client.Connect(clientId, username, password);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { AllPositionsTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        /*messageDictionary = new Dictionary<int, MqttMessage>();
        messageHistory = new int[numberOfMessagesToStore]; //these are keys to dictionary
        for (int x = 0; x < numberOfMessagesToStore; x++)
        {
            messageHistory[x] = Int32.MaxValue;//initialize values to max
        }*/
        
        StartCoroutine("SendMessage");//this starts the message loop with frequency sendFrequency seconds.
    }
    public string AllPositionsTopic
    {
        get { return topicPrefix + "/+/positions-ts"; }
    }
    public string SelfPositionsTopic
    {
        get { return topicPrefix + "/" + clientId + "/positions"; }
    }
    public string OpponentPositionsTopic
    {
        get { return topicPrefix + "/" + otherClientId + "/positions"; }
    }

    void client_MqttMsgDisconnected(object self, System.EventArgs e)
    {
        Debug.Log("Disconnected");
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string response = System.Text.Encoding.UTF8.GetString(e.Message);
        //Debug.Log("Received: " + response);
        myMessages = JsonUtility.FromJson<MWrapper>(response);
        foreach (MqttMessage position in myMessages.positions)//for each message received
        {
            if (position.messageType == "position")
            {
                if (position.objectType == "Asteroid")
                {
                    //asteroid - do something with this object
                }
                else if (position.objectType == "Ship")
                {

                }
                else if (position.objectType == "Mine")
                {

                }
            }
            else if (position.messageType == "join")
            {
                if (!joined) 
                {
                    if (myMessages.clientId == clientId)
                    { 
                        myJoinTime = position.serverUTCTime;
                        //Debug.Log("myJoinTime = " + myJoinTime);
                        if (theirJoinTime > 0)
                        {
                            PickMaster();
                        }
                    
                    }
                    else if (otherClientId != myMessages.clientId)
                    {
                        otherClientId = myMessages.clientId;
                        Debug.Log("our clientId " + clientId+" their clientId " + otherClientId);
                        theirJoinTime = position.serverUTCTime;
                        //Debug.Log("theirJoinTime = " + theirJoinTime);
                        if (myJoinTime > 0)
                        {
                            PickMaster();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("received message type: " + position.messageType);
            }
            /*messageHistory[messageHistoryPosition++] = messageId++;  //increment iterator and message count (until we receive from server)
            if (messageHistoryPosition == numberOfMessagesToStore)
            {
                messageHistoryPosition = 0;
            }
            messageDictionary.Add(messageId, position);//add the MqttMessage to dict, referenced by the key id*/

        }
    }

    private void PickMaster()
    {
        if (myJoinTime < theirJoinTime)
        {
            Debug.Log("I am master");
            joined = true;
            client.Unsubscribe(new string[] { AllPositionsTopic });
            client.Subscribe(new string[] { OpponentPositionsTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }
        else if (myJoinTime > theirJoinTime)
        {
            Debug.Log("I am slave");
            joined = true;
            client.Unsubscribe(new string[] { AllPositionsTopic });
            client.Subscribe(new string[] { OpponentPositionsTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }
        else
        {
            Debug.Log("tie, reset");
            myJoinTime = theirJoinTime = 0;
        }
    }

    /*private void MoveTarget()
    {
        int mostRecentIndex = FindOldest();
        if (mostRecentIndex >= 0)
        {
            MqttMessage message = null;
            if (messageDictionary.TryGetValue(messageHistory[mostRecentIndex], out message))
            {
                float x = message.x;
                float y = message.y;
                player2.transform.position = new Vector2(x, y);
                messageDictionary.Remove(messageHistory[mostRecentIndex]);
                messageHistory[mostRecentIndex] = Int32.MaxValue;
                //Debug.Log("Received x: "+ x.ToString() + " y: "+y.ToString());
            }
            else
            {
                //Debug.Log("key " + mostRecentIndex + "not found.");
            }
        }
    }*/

    // Update is called once per frame
    void Update()
    {

    }
    
  

    private IEnumerator SendMessage()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(sendFrequency);
            if (joined)//send our positions every sendFrequency seconds
            {
                SendPackets(ObjectPooler.Instance.ManagedObjects);
            }
            else//send a new join every second until joined
            {
                yield return new WaitForSeconds(1);
                SendaPacket("join");
            }
        }
    }


    /*public void SendEventPackets(Dictionary<string, GameObject> events)//not finished
    {
        string positions = "[";
        foreach (KeyValuePair<string, GameObject> entry in events)
        {
            //Debug.Log("key " + entry.Key + " value: " + entry.Value);
            positions += "{  \"type\":" + entry.Key + "\"objectId\":" + entry.Key + ",\"x\":" + entry.Value.transform.position.x + ",\"y\":" + entry.Value.transform.position.y + "}";
        }
        positions += "]";
        //Debug.Log("positions: " + positions);
        string payload = "{ \"clientId\": \"" + clientId + "\",\"positions\":" + positions + "}";
        Debug.Log("payload: " + payload);
        client.Publish(SelfPositionsTopic,
                System.Text.Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
    }*/

    public void SendPackets(Dictionary<string, GameObject> objects)//sends many position packets
    {
        string positions = "[";
        int size = objects.Count;
        int loopPosition = 0;
        foreach (KeyValuePair<string, GameObject> entry in objects)
        {
            MqttMessage message = new MqttMessage//using this class to build JSON strings, use fields as needed
            {
                messageType = "position",
                objectType = entry.Value.GetComponent<NetworkID>().Type,
                objectId = entry.Key,
                id = messageId++
            };
            positions += message.SaveToString();//append entry
            if (loopPosition++ < size - 1)//add commas to string if necessary
            {
                positions += ",";
            }
        }
        positions += "]";
        string payload = "{ \"clientId\": \"" + clientId + "\",\"positions\":" + positions + "}";
        client.Publish(SelfPositionsTopic,
                System.Text.Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
    }

    public void SendaPacket(string input)//sends a basic message packet
    {
        string positions = "[";

        MqttMessage message = new MqttMessage//using this class to build JSON strings
        {
            messageType = input,
            objectType = "joinRequest",
            objectId = "none",
            x = 0f,
            y = 0f,
            id = messageId++
        };
        positions += message.SaveToString();
        positions += "]";
        string payload = "{ \"clientId\": \"" + clientId + "\",\"positions\":" + positions + "}";
        //Debug.Log(payload);
        client.Publish(SelfPositionsTopic,
                System.Text.Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
    }
    
    /*private int FindOldest()//find oldest id, currently unused
    {
        int min = Int32.MaxValue;
        for (int x = 0; x < numberOfMessagesToStore; x++)
        {
            if (messageHistory[x] < min)
            {
                min = x;
            }
        }
        if (min < Int32.MaxValue)
        {
            return min;
        }
        else
            return -1;
    }*/
}

[Serializable]
public class MWrapper
{
    public string clientId;
    public List<MqttMessage> positions;
}

[Serializable]
public class MqttMessage
{
    public string messageType;
    public string objectType;
    public string objectId;    
    public float x;
    public float y;
    public int id;
    public long serverUTCTime;
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}