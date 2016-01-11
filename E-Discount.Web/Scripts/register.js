var callbackUrl = './E-Discount-Finder.ashx';
var p_markerIcon = './images/p-marker.png';
var placeSearch, autocomplete, map, marker, initialLocation;
var componentForm = {
    street_number: 'short_name',
    route: 'long_name',
    locality: 'long_name',
    administrative_area_level_1: 'short_name',
    country: 'long_name',
    postal_code: 'short_name'
};

function initialize() {
    initialLocation = new google.maps.LatLng(42.211, -71.011);
    var mapOptions = {
        center: initialLocation,
        panControl: false,
        streetViewControl: false,
        disableDoubleClickZoom: true,
        scrollwheel: false,
        scaleControl:false,
        zoom: 13
    };
    //getCenter()
    map = new google.maps.Map(document.getElementById('map-canvas'),
      mapOptions);
    // Create the autocomplete object, restricting the search
    // to geographical location types.
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autoAddress')),
        { types: ['geocode'] });

    marker = new google.maps.Marker({
        map: map,
        anchorPoint: new google.maps.Point(0, -29),
        animation: google.maps.Animation.DROP,
        icon: p_markerIcon
    });
    autocomplete.bindTo('bounds', map);
    // When the user selects an address from the dropdown,
    // populate the address fields in the form.
    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        fillInAddress();
    });
}

// [START region_fillform]
function fillInAddress() {
    marker.setVisible(false);
    // Get the place details from the autocomplete object.
    var place = autocomplete.getPlace();

    if (!place.geometry) {
        return;
    }
    if (place.geometry.viewport) {
        map.fitBounds(place.geometry.viewport);
    } else {
        initialLocation = place.geometry.location;
        map.setCenter(initialLocation);
        map.setZoom(17);  // Why 17? Because it looks good.
    }
    marker.setPosition(place.geometry.location);
    marker.setVisible(true);
    for (var component in componentForm) {
        document.getElementById(component).value = '';
        document.getElementById(component).disabled = false;
    }

    // Get each component of the address from the place details
    // and fill the corresponding field on the form.
    for (var i = 0; i < place.address_components.length; i++) {
        var addressType = place.address_components[i].types[0];
        if (componentForm[addressType]) {
            var val = place.address_components[i][componentForm[addressType]];
            document.getElementById(addressType).value = val;
        }
    }
}
// [END region_fillform]

// [START region_geolocation]
// Bias the autocomplete object to the user's geographical location,
// as supplied by the browser's 'navigator.geolocation' object.
function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var geolocation = new google.maps.LatLng(
                position.coords.latitude, position.coords.longitude);
            var circle = new google.maps.Circle({
                center: geolocation,
                radius: position.coords.accuracy
            });
            autocomplete.setBounds(circle.getBounds());
        });
    }
}


function registerOwnerSubmit() {
    $(".error").text("");
    var validation = 1;
    var bname = $("#bname").val();
    var street_number = $("#street_number").val();
    var route = $("#route").val();
    var city = $("#locality").val();
    var state = $("#administrative_area_level_1").val();
    var postal_code = $("#postal_code").val();
    var country = $("#country").val();
    var fname = $("#fname").val();
    var lname = $("#lname").val();
    var loginID = $("#loginID").val();
    var password = $("#password").val();
    var phone = $("#phone").val();
    var price = $("#price").val();
    var website = $("#website").val();
    var email = $("#email").val();
    if (initialLocation.longitude == -71.011 && initialLocation.latitude == 42.211)//LatLng(42.211, -71.011) is the default point.
    {
        alert("Please enter your business location.");
    }
    if (bname == null || bname == "") {
        $("#error_bname").text("*"); validation = 0;
    }
    if (fname == null || fname == "") {
        $("#error_fname").text("*Required"); validation = 0;
    }
    if (lname == null || lname == "") {
        $("#error_lname").text("*Required"); validation = 0;
    }
    if (loginID == null || loginID == "") {
        $("#error_loginID").text("*Required"); validation = 0;
    }
    if (password == null || password == "") {
        $("#error_password").text("*Required"); validation = 0;
    }
    if (email == null || email == "") {
        $("#error_email").text("*Required"); validation = 0;
    }
    if (!$("#email")[0].checkValidity()) {
        $("#error_email").text("*Please include an '@' in the email address."); validation = 0;
    }
    if(validation) {
        $.get(callbackUrl + "?query=registerAsOwner&longitude=" + initialLocation.D + "&latitude=" + initialLocation.k + "&bname=" + bname + "&street_number=" + street_number + "&route=" + route + "&city=" + city + "&state=" + state + "&postal_code=" + postal_code + "&country=" + country + "&price=" + price + "&fname=" + fname + "&lname=" + lname + "&loginID=" + loginID + "&password=" + password + "&phone=" + phone + "&email=" + email + "&website=" + website, userCallback);
    }
}

function registerSubmit() {
    $(".error").text("");
    var validation = 1;
    var fname = $("#fname").val();
    var lname = $("#lname").val();
    var loginID = $("#loginID").val();
    var password = $("#password").val();
    var email = $("#email").val();
    if (fname == null || fname == "") {
        $("#error_fname").text("*Required"); validation = 0;
    }
    if (lname == null || lname == "") {
        $("#error_lname").text("*Required"); validation = 0;
    }
    if (loginID == null || loginID == "") {
        $("#error_loginID").text("*Required"); validation = 0;
    }
    if (password == null || password == "") {
        $("#error_password").text("*Required"); validation = 0;
    }
    if (email == null || email == "") {
        $("#error_email").text("*Required"); validation = 0;
    }
    if (!$("#email")[0].checkValidity()) {
        $("#error_email").text("*Please include an '@' in the email address."); validation = 0;
    }
    if (!validation) {
        return false;
    }
    if (validation) {
        $.get(callbackUrl + "?query=registerAsOwner&fname=" + fname + "&lname=" + lname + "&loginID=" + loginID + "&password=" + password+ "&email=" + email, userCallback);
    }
}
function loginSubmit() {
    $(".error").text("");
    var validation = 1;
    var loginID = $("#loginID").val();
    var password = $("#password").val();
    if (loginID == null || loginID == "") {
        $("#error_loginID").text("*Required"); validation = 0;
    }
    if (password == null || password == "") {
        $("#error_password").text("*Required"); validation = 0;
    }
    if (validation) {
        $.get(callbackUrl + "?query=login&loginID=" + loginID + "&password=" + password , userCallback);
    }
}
function modifyOwnerSubmit() {
    $(".error").text("");
    var validation = 1;
    var bname = $("#bname").val();
    var street_number = $("#street_number").val();
    var route = $("#route").val();
    var city = $("#locality").val();
    var state = $("#administrative_area_level_1").val();
    var postal_code = $("#postal_code").val();
    var country = $("#country").val();
    var fname = $("#fname").val();
    var lname = $("#lname").val();
    var loginID = $("#loginID").val();
    var password = $("#password").val();
    var phone = $("#phone").val();
    var price = $("#price").val();
    var website = $("#website").val();
    var email = $("#email").val();
    if (initialLocation.longitude == -71.011 && initialLocation.latitude == 42.211)//LatLng(42.211, -71.011) is the default point.
    {
        alert("Please enter your business location.");
    }
    if (bname == null || bname == "") {
        $("#error_bname").text("*"); validation = 0;
    }
    if (fname == null || fname == "") {
        $("#error_fname").text("*Required"); validation = 0;
    }
    if (lname == null || lname == "") {
        $("#error_lname").text("*Required"); validation = 0;
    }
    if (loginID == null || loginID == "") {
        $("#error_loginID").text("*Required"); validation = 0;
    }
    if (password == null || password == "") {
        $("#error_password").text("*Required"); validation = 0;
    }
    if (email == null || email == "") {
        $("#error_email").text("*Required"); validation = 0;
    }
    if (!$("#email")[0].checkValidity()) {
        $("#error_email").text("*Please include an '@' in the email address."); validation = 0;
    }
    if (validation) {
        $.get(callbackUrl + "?query=modifyOwner&longitude=" + initialLocation.D + "&latitude=" + initialLocation.k + "&bname=" + bname + "&street_number=" + street_number + "&route=" + route + "&city=" + city + "&state=" + state + "&postal_code=" + postal_code + "&country=" + country + "&price=" + price + "&fname=" + fname + "&lname=" + lname + "&loginID=" + loginID + "&password=" + password + "&phone=" + phone + "&email=" + email + "&website=" + website, userCallback);
    }
}
function userCallback(result, status) {
    window.parent.tb_remove();
    if (status == 'success') {
        var u = JSON.parse(result);
        $("#btnLogoff", window.parent.document).removeClass("hidden");
        var loginHtml = "";
        if (u.Role = 0) {
            loginHtml = '<a href="ModifyUser.html?keepThis=true&TB_iframe=false&height=600&width=900" title="Modify User" class="thickbox btn-xs btn-link">' + u.Name + '</a>';
        }
        else {
            loginHtml = '<a href="ModifyOwner.html?keepThis=true&TB_iframe=false&height=700&width=900" title="Modify Owner" class="thickbox btn-xs btn-link">' + u.Name + '</a>';
        }
        $('#login', window.parent.document).html(loginHtml);
        window.parent.tb_init('a.thickbox');
    }
}
function getOwner()
{
    $.get(callbackUrl + "?query=getOwner", getOwnerCallback);
}
function getOwnerCallback(result, status) {
    if (status == 'success') {
        var strs = result.split("___");
        var user = JSON.parse(strs[0]);
        var store = JSON.parse(strs[1]);
        $("#sid").val(store.STORE_ID);
        $("#bname").val(store.NAME);
        $("#autoAddress").val(store.ADDRESS_LINE);
        var route = $("#route").val(store.ADDRESS_LINE);
        var city = $("#locality").val(store.CITY);
        var state = $("#administrative_area_level_1").val(store.STATE_PROVINCE_REGION);
        var postal_code = $("#postal_code").val(store.POSTAL_CODE);
        var country = $("#country").val(store.COUNTRY);
        $("#fname").val(user[0].FIRST_NAME);
        $("#lname").val(user[0].LAST_NAME);
        $("#loginID").val(user[0].USER_ID);
        $("#password").val(user[0].PASSWORD);
        $("#phone").val(store.PHONE);
        $("#price").val(store.PRICE_LEVEL);
        $("#website").val(store.WEBSITE);
        $("#email").val(user[0].EMAIL);
    }
}
function checkDiscountCallback(result, status) {
    if (status == 'success') {
        $("#discountStatus").text("success.");
    }
}