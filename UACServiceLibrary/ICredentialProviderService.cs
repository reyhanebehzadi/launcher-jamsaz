using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace UACServiceLibrary
{
    [ServiceContract]
    public interface ICredentialProviderService
    {
        [OperationContract(IsOneWay = true)]
        void AddCredential(Credential credential);

        [OperationContract]
        Credential GetUserCredential();
    }
}
