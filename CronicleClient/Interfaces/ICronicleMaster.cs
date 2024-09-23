using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CronicleClient.Interfaces
{
    public interface ICronicleMaster
    {
        Task<bool?> GetMasterState(CancellationToken cancellationToken = default);
        Task UpdateMasterState(bool newMasterState, CancellationToken cancellationToken = default);
    }

}
