namespace Service.Accounts
{
    public record GetAccountInformationRequest;
    public record GetAccountInformationResponse(AccountDto Account);

    public record AccountDto(string GivenName, string SurName, string Email, string[] Roles, string[] FeatureFlags)
    {
        public string Name { get; } = $"{GivenName} {SurName}".Trim();
    }
}
