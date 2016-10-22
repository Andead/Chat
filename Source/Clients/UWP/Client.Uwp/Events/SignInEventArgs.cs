using System;

namespace Andead.Chat.Client.Uwp
{
    public class SignInEventArgs : EventArgs
    {
        public SignInEventArgs(SignInResult result, ChatViewModel chatViewModel)
        {
            Result = result;
            ChatViewModel = chatViewModel;

            if (chatViewModel != null)
            {
                chatViewModel.SignInResult = result;
            }
        }

        public SignInResult Result { get; }

        public ChatViewModel ChatViewModel { get; }
    }
}