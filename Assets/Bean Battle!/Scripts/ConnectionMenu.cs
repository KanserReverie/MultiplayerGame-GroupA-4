using kcp2k;
using Mirror;
using Beanbattle.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Beanbattle
{
	public class ConnectionMenu : MonoBehaviour
	{
		private CustomNetworkManager networkManager;
		private KcpTransport transport;
		
		[SerializeField] private Button hostButton;
		[SerializeField] private InputField inputField;
		[SerializeField] private Button connectButton;

		[Space] [SerializeField] private DiscoveredGame discoveredGameTemplate;

		private readonly Dictionary<IPAddress, DiscoveredGame> discoveredGames = new Dictionary<IPAddress, DiscoveredGame>();

		private void Start()
		{
			networkManager = CustomNetworkManager.Instance;
			transport = Transport.activeTransport as KcpTransport;

			hostButton.onClick.AddListener(OnClickHost);
			inputField.onEndEdit.AddListener(OnEndEditAddress);
			connectButton.onClick.AddListener(OnClickConnect);

			CustomNetworkDiscovery discovery = networkManager.discovery;
			discovery.onServerFound.AddListener(OnFoundServer);
			discovery.StartDiscovery();
		}

		private void OnClickHost() => networkManager.StartHost();
		
		private void OnEndEditAddress(string _value) => networkManager.networkAddress = _value;
		
		private void OnClickConnect()
		{
			string address = inputField.text;
			ushort port = 7777;
			if(address.Contains(":"))
			{
				string portID = address.Substring(address.IndexOf(":", StringComparison.Ordinal) + 1);
				port = ushort.Parse(portID);
				address = address.Substring(0, address.IndexOf(":", StringComparison.Ordinal));
			}

			if(!IPAddress.TryParse(address, out IPAddress _))
			{
				Debug.LogError($"Invalid IP: {address}");
				address = "localhost";
			}

			((KcpTransport)Transport.activeTransport).Port = port;
			networkManager.networkAddress = address;
			networkManager.StartClient();
		}
		
		private void OnFoundServer(DiscoveryResponse _response)
		{
			// Have we recieved a server that is broadcasting on the network that we haven't already found
			if(!discoveredGames.ContainsKey(_response.EndPoint.Address))
			{
				// We haven't found this game already, so make the gameObject
				DiscoveredGame game = Instantiate(discoveredGameTemplate, discoveredGameTemplate.transform.parent);
				game.gameObject.SetActive(true);

				// Setup the game using the response and add it to the dictionary
				game.Setup(_response, networkManager, transport);
				discoveredGames.Add(_response.EndPoint.Address, game);
			}
		}
	}
}