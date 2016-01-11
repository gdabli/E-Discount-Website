
var callbackUrl = './E-Discount-Finder.ashx';
$(document).ready(function () {
    $.get(callbackUrl + "?query=category", loadCategory);
    $.get(callbackUrl + "?query=checkLogin", checkLoginCallback);
});
function finder(fun_name, m_data) {
    $.ajax({
        type: "GET",
        url: callbackUrl,
        data: m_data,
        cache: false,
        dataType: "json",
        success: function (result) {
            if (result != 'none') {
                // Create markers on map
            }
        },
        error: function () {
            alert(result.Error);
        }
    });
}
function loadCategory(data, status)
{
    var catrgories = JSON.parse(data);
    for (var i in catrgories) {
        $("#drop_category").append('<option value="' + catrgories[i].CATEGORY_ID + '">' + catrgories[i].NAME.toUpperCase() + '</option>');
    }
}
function checkLoginCallback(result, status) {
    if (status == 'success') {
        tb_remove();
        var u = JSON.parse(result);
        $("#btnLogoff").removeClass("hidden");
        var loginHtml = "";
        if (u.Role = 0) {
            loginHtml = '<a href="ModifyUser.html?keepThis=true&TB_iframe=false&height=600&width=900" title="Modify User" class="thickbox btn-xs btn-link">' + u.Name + '</a>';
        }
        else {
            loginHtml = '<a href="ModifyOwner.html?keepThis=true&TB_iframe=false&height=700&width=900" title="Modify Owner" class="thickbox btn-xs btn-link">' + u.Name + '</a> ';
        }
        $('#login', window.parent.document).html(loginHtml);
        tb_init('a.thickbox')
    }
}
function feedbackCallback(result, status) {
    if (status == 'success') {
        $('#feedbackStatus').text(result);
        $('#feedback').addClass('hidden')
    }
}
function logoffCallback(result, status) {
    if (status == 'success') {
        location.reload(true);
    }
}