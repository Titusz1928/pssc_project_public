using Lab2.API.Clients;
using Microsoft.AspNetCore.Mvc;

//NOT USED

namespace Lab2.API.Controllers;

public class MyController : ControllerBase
{
    private readonly MyApiClient _myApiClient;

    public MyController(MyApiClient myApiClient)
    {
        _myApiClient = myApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var data = await _myApiClient.GetDataAsync();
        return Ok(data);
    }
}