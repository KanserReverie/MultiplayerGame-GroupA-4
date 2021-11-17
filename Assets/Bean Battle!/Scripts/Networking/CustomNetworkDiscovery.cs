using kcp2k;

using Mirror;
using Mirror.Discovery;

using System;
using System.Net;

using UnityEngine;
using UnityEngine.Events;

namespace Beanbattle.Networking
{
	public class DiscoveryRequest : NetworkMessage { }

	public class DiscoveryResponse : NetworkMessage
	{
		// The server that sent the message
		// This is a property so that it is not serialized but the client
		// fills this up after we recieve it
		public IPEndPoint EndPoint { get; set; }

		public Uri uri;

		public ushort port;

		public long serverId;
	}

	[Serializable]
	public class ServerFoundEvent : UnityEvent<DiscoveryResponse> { }

	public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
	{
	#region Server

		public long ServerId { get; private set; }

		[Tooltip("Transport to be advertised during discovery.")]
		public Transport transport;

		[Tooltip("Invoked when a server is found")] public ServerFoundEvent onServerFound = new ServerFoundEvent();

		// Start is called before the first frame update
		public override void Start()
		{
			ServerId = RandomLong();
			
			// If the transport wasn't set in the inspector, use the active one
			// Transport.activeTransport is set in Awake of Transport components.
			if(transport == null)
				transport = Transport.activeTransport;
			
			base.Start();
		}

		/// <summary> Reply to the client to inform it of this server </summary>
		/// <remarks>
		/// Override if you wish to ignore server requests based on
		/// custom criteria such as language, full server game mode or difficulty
		/// </remarks>
		/// <param name="_request">Request coming from client</param>
		/// <param name="_endpoint">Address of the client that sent the request</param>
		protected override DiscoveryResponse ProcessRequest(DiscoveryRequest _request, IPEndPoint _endpoint)
		{
			try
			{
				// This is just an example reply message, you could add anything here about the match.
				// ie game name, game mode, host name, language, ping
				// This has a chance to throw an exception if the transport doesn't support network discovery.
				return new DiscoveryResponse()
				{
					serverId = ServerId,
					uri = transport.ServerUri(),
					port = ((KcpTransport)transport).Port
				};
			}
			catch(NotImplementedException e)
			{
				Debug.LogException(e, gameObject);
				throw;
			}
		}

		/// <summary> Create a message that will be broadcasted on the network to discover servers </summary>
		/// <remarks>
		/// Override if you wish to include additional data in the discovery message
		/// such as desired game mode, language, difficulty, etc... </remarks>
		/// <returns>An instance of ServerRequest with data to be broadcasted</returns>
		protected override DiscoveryRequest GetRequest() => new DiscoveryRequest();
		
	#endregion

	#region Client

		/// <summary> Process the answer from a server </summary>
		/// <remarks>
		/// A client receives a reply from a server, this method processes the
		/// reply and raises an event
		/// </remarks>
		/// <param name="_response">Response that came from the server</param>
		/// <param name="_endpoint">Address of the server that replied</param>
		protected override void ProcessResponse(DiscoveryResponse _response, IPEndPoint _endpoint)
		{
		// James doesn't fully understand this code, but knows this is just something that needs to be done.
		#region WTF

			// We recieved a message from the remote endpoint
			_response.EndPoint = _endpoint;
			
			// Although we got a supposedly valid url we may not be able to resolve
			// the provided host
			// However we know the real ip address of the server because we just recieved a
			// packet from it, so use that as the host uri
			UriBuilder realUri = new UriBuilder(_response.uri)
			{
				Host = _response.EndPoint.Address.ToString()
			};
			_response.uri = realUri.Uri;

		#endregion
			
			onServerFound.Invoke(_response);
		}

	#endregion
	}
}