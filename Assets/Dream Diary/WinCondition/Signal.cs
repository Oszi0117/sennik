using System;

public class Signal {
    public event Action Receptors;

    public void Broadcast() {
        Receptors?.Invoke();
    }

    public void Subscribe(Action subscriber) {
        Receptors += subscriber;
    }

    public void Unsubscribe(Action subscriber) {
        Receptors -= subscriber;
    }
}