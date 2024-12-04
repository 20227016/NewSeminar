using Fusion;
using System.Collections.Generic;

public interface IClickEvent
{
    void VoidClickEventAction();
    void StringClickEventAction(string roomName);

    void NetworkClickEventAction(NetworkRunner runner, List<SessionInfo> sessionList);
}