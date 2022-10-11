using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace de.jmu.ge.viavr.supervisorintegration {
    
    public class SupervisorLogger : MonoBehaviour {
        public string messageFormat;
        private const string Identifier = "LogEvent";

        public void SendToSupervisor() {
            string messageJson = ConvertToJson(
                new KeyValuePair<string, string>("time", "[" + DateTime.Now.ToShortTimeString() + "]"),
                new KeyValuePair<string, string>("logMessage", InferMessage(messageFormat)));
            
            // RestRequester.GetInstance().MakePutRequest(
            //     Identifier,
            //     _ => {},
            //     messageJson,
            //     "application/json"
            // );
        } 

        // This works because C# always has one string before the split char and one afterwards.
        // But not really foolproof => Doesn't check for closing brackets... 
        private string InferMessage(string messageBlueprint) {
            var messageParts = messageBlueprint.Split('{','}');
            StringBuilder newMessage = new ();
            for(int partIndex = 0; partIndex < messageParts.Length; partIndex++) {
                /*if(partIndex % 2 == 0) {
                    newMessage.Append(mappedGameStates[messageParts[partIndex]]);
                }
                else {*/
                newMessage.Append(messageParts[partIndex]);
                //}
            }
            return newMessage.ToString();
        }

        private string ConvertToJson(params KeyValuePair<string, string>[] jsonFields) {
            StringBuilder json = new();
            json.Append("{ ");
            json.Append(
                string.Join(" , ",
                    jsonFields.Select(field => "\"" + field.Key + "\": \"" + field.Value + "\"")
                )
            );
            json.Append(" }");
            return json.ToString();
        }
    }
}