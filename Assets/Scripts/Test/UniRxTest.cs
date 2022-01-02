using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniRxTest : MonoBehaviour {

    void Start () {
        // var asdf = UniRx.Observable.Create<Event>(KLOASDASDFFASIFG);
    }

    private IDisposable KLOASDASDFFASIFG(IObserver<Event> arg)
    {
        throw new NotImplementedException();
    }

    void FuncTest (System.Func<string, int> myFunc){
        myFunc("hello");
    }

    MyDisposable DoTheSubscriptionPlease (EventObserver obs) {
        return default;
    }

    void OnGUI () {
        var curr = Event.current;
    }

    class EventObserver : System.IObserver<Event> {

        void IObserver<Event>.OnCompleted () {
            throw new NotImplementedException();
        }

        void IObserver<Event>.OnError (Exception error) {
            throw new NotImplementedException();
        }

        void IObserver<Event>.OnNext (Event value) {
            throw new NotImplementedException();
        }
    }

    class MyDisposable : System.IDisposable {

        void IDisposable.Dispose () {
            throw new NotImplementedException();
        }

    }

}
