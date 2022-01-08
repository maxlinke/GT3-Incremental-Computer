public class UpdateCycleObserver : System.IObserver<long> {

    private readonly System.Action onNext;

    public UpdateCycleObserver (System.Action onUpdate) {
        onNext = onUpdate;
    }

    void System.IObserver<long>.OnCompleted () { }

    void System.IObserver<long>.OnError (System.Exception error) { }

    void System.IObserver<long>.OnNext (long value) {
        onNext.Invoke();
    }

}
