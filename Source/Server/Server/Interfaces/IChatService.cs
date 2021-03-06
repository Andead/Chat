﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace Andead.Chat.Server
{
    [ServiceContract(CallbackContract = typeof(IChatClient))]
    public interface IChatService
    {
        [OperationContract]
        SignInResponse SignIn(SignInRequest request);

        [OperationContract]
        void SignOut();

        [OperationContract]
        SendMessageResponse SendMessage(SendMessageRequest request);

        [OperationContract]
        int? GetOnlineCount();

        [OperationContract]
        ReadOnlyCollection<string> GetNamesOnline();
    }
}