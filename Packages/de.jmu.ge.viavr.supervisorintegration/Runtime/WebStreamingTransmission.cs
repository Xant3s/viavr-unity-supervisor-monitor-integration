using System.Net;
using System.Threading;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Signaling;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    public class WebStreamingTransmission {
        private RenderStreaming renderStreamer;

        public void StartTransmission(IPAddress ipAddress, Transform gameObjectTransform) {
            ISignaling signaling = new WebSocketSignaling($"ws://{ipAddress}", 5.0f, SynchronizationContext.Current);
            SignalingHandlerBase handlerBase = gameObjectTransform.GetComponentInChildren<Broadcast>();
            renderStreamer = gameObjectTransform.GetComponentInChildren<RenderStreaming>();
            renderStreamer.Run(true, signaling, new []{handlerBase});
        }
        
        public void StopTransmission() {
            renderStreamer.Stop();
        }
    }
}