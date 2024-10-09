using System;
using System.Collections.Generic;
using System.Text;

namespace CronicleClient.Interfaces
{
    public interface ICronicleClient
    {
        ICronicleEvent Event { get; }
        ICronicleJob Job { get; }
        ICronicleMaster Master { get; }
    }

}
