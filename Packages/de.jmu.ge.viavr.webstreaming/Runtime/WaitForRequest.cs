using System;
using System.Threading.Tasks;

namespace de.jmu.ge.viavr.webstreaming {
    public class WaitForRequest<T> {
        private readonly Func<Task<T>> request;
        private readonly Func<T, bool> predicate;
        private readonly int frequency;
        private readonly int timeout;
        
        public T Result { get; private set; }


        public WaitForRequest(Func<Task<T>> request, Func<T, bool> predicate, int frequency = 500, int timeout = -1) {
            this.timeout = timeout;
            this.frequency = frequency;
            this.predicate = predicate;
            this.request = request;
        }

        public async Task WaitUntil() {
            var waitTask = Task.Run(async () => {
                while(!await Test()) await Task.Delay(frequency);
            });

            if(waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout))) {
                throw new TimeoutException();
            }
        }

        private async Task<bool> Test() {
            Result = await request();
            return predicate(Result);
        }
    }
}