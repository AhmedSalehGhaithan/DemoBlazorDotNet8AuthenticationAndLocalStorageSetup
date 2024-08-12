using System.Text.Json;

namespace BlazorWebAssemblyApp.Authentication
{
    public static class SerializeOrDeSerialize
    {
        public static string Serialize(AuthenticationModel model) => JsonSerializer.Serialize(model);

        public static AuthenticationModel Deserialize(String serializeString) => JsonSerializer.Deserialize<AuthenticationModel>(serializeString)!;
    }
}
