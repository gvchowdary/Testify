function LoadLoginPartialView(userType)
{
    var id = userType;
    var loginDiv = $("#logindisplay");
    $.ajax({
        cache: false,
        type: "GET",
        url: "/Account/LogInInfo",
        data: { "userType": id },
        success: function (data) {

            loginDiv.html('');
            loginDiv.html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve user details.');
        }
    });
}