using Bunit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VoteScale.Web.Auth;
using VoteScale.Web.Components.Pages;

namespace VoteScale.Web.Tests;

public class LoginTests : BunitContext
{
    [Fact]
    public void Login_ShouldCallToastService_WhenCredentialsAreInvalid()
    {
        // Arrange
        Services.AddScoped<AuthenticationStateProvider, SimpleAuthProvider>();
        Services.AddAuthorization();

        // 1. Criamos um "Mock" (um objeto espião) do ToastService
        var toastMock = new Mock<ToastService>();
        Services.AddSingleton(toastMock.Object);

        var cut = Render<Login>();

        // Act
        cut.Find("input[placeholder='admin']").Change("errado");
        cut.Find("input[type='password']").Change("errado");
        cut.Find("button[type='submit']").Click();

        // Assert
        // 2. Perguntamos ao espião: "O método ShowError foi chamado com alguma mensagem?"
        toastMock.Verify(t => t.ShowError("Usuário ou senha inválidos."), Times.Once);
    }
}