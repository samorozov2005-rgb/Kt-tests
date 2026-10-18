using BudgetCalculator.Api.Controllers;
using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    [Fact]
    public void Calculate_ValidRequestWithoutInsurance_ReturnsCorrectTotal()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = false
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(500m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_RequestWithInsurance_ReturnsTotalWithFivePercentInsurance()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = true
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(525m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_NegativeDays_ReturnsBadRequest()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = -1,
            IncludeInsurance = false
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", result.Value);
    }

    [Fact]
    public void Calculate_ZeroDays_ReturnsBadRequest()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 0,
            IncludeInsurance = false
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", result.Value);
    }

    [Fact]
    public void Calculate_ZeroDailyCost_ReturnsZero()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 0m,
            Days = 5,
            IncludeInsurance = false
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(0m, GetTotalAmount(result));
    }

    [Fact]
    public void Calculate_DecimalDailyCost_ReturnsCorrectAmount()
    {
        // Arrange
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100.50m,
            Days = 3,
            IncludeInsurance = false
        };

        // Act
        var actionResult = controller.Calculate(request);

        // Assert
        var result = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(301.50m, GetTotalAmount(result));
    }

    private static decimal GetTotalAmount(OkObjectResult result)
    {
        var totalAmountProperty = result.Value?.GetType().GetProperty("TotalAmount");
        Assert.NotNull(totalAmountProperty);

        return Assert.IsType<decimal>(totalAmountProperty.GetValue(result.Value));
    }
}
