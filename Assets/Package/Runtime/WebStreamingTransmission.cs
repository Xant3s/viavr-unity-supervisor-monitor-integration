using System.Linq;
using System.Threading;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Signaling;
using UnityEngine;

namespace Package.Runtime {
    public class WebStreamingTransmission : ServerTransmission {
        private RenderStreaming renderStreamer;

        public void StartTransmission(string receivedMessage, Transform gameObjectTransform) {
            string sanitizedWsAddress = receivedMessage.Split(' ').ToList().Last();
            ISignaling signaling = new WebSocketSignaling($"ws://{sanitizedWsAddress}", 5.0f, SynchronizationContext.Current);
            SignalingHandlerBase handlerBase = gameObjectTransform.GetComponent<Broadcast>();
            renderStreamer = gameObjectTransform.GetComponent<RenderStreaming>();
            renderStreamer.Run(true, signaling, new []{handlerBase});
        }
        
        public void StopTransmission() {
            renderStreamer.Stop();
        }
    }
}