using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace UACServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    public class CredentialProviderService : ICredentialProviderService
    {
        private Credential credential;

        public void AddCredential(Credential credential)
        {
            this.credential = credential;
        }

        public Credential GetUserCredential()
        {
            return this.credential;
        }
    }
}
