using System;
using System.Collections.Generic;
using System.Text;

namespace CronicleClient.Interfaces
{
    public interface ICronicleClient
    {
        CronicleEvent Event { get; }
        CronicleJob Job { get; }
        CronicleMaster Master { get; }
    }

}
