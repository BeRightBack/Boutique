﻿namespace Boutique.Models.ManageViewModels;
public class TwoFactorAuthenticationViewModel
{
    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    public string StatusMessage { get; set; }
}
