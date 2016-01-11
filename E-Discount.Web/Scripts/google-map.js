var map, places, initialLocation, autocomplete;
var markers = [];
var stores = [];
var recommendation;
var browserSupportFlag = new Boolean();
var g_markerIcon = './images/dg-marker.png';
var p_markerIcon = './images/p-marker.png';
var callbackUrl = './E-Discount-Finder.ashx';
var currentMarker;
var hostnameRegexp = new RegExp('^https?://.+?/');
function initialize() {
    initialLocation = new google.maps.LatLng(42.286109924316406, -71.064590454101562);
    var mapOptions = {
        center: initialLocation,
        zoom: 13,
        mapTypeControl: true,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
            position: google.maps.ControlPosition.BOTTOM_CENTER
        },
        scaleControl: false,
        zoomControl: false,
        panControl: false,
        streetViewControl: false,
    };
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
    autocomplete = new google.maps.places.Autocomplete((document.getElementById('locationInput')), { types: ['geocode'] });
    autocomplete.bindTo('bounds', map);
    places = new google.maps.places.PlacesService(map);
    infoWindow = new google.maps.InfoWindow({
        content: document.getElementById('info-content')
    });

    // Try W3C Geolocation (Preferred)
    if (navigator.geolocation) {
        browserSupportFlag = true;
        navigator.geolocation.getCurrentPosition(function (position) {
            initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            map.setCenter(initialLocation);
            if (currentMarker != null) {
                currentMarker.setMap(null);
            }
            currentMarker = new google.maps.Marker({
                position: initialLocation,
                animation: google.maps.Animation.DROP,
                icon: p_markerIcon
            });
            currentMarker.setMap(map);
        }, function () {
            handleNoGeolocation(browserSupportFlag);
        });
    }
        // Browser doesn't support Geolocation
    else {
        browserSupportFlag = false;
        handleNoGeolocation(browserSupportFlag);
    }
    function handleNoGeolocation(errorFlag) {
        if (errorFlag == true) {
            alert("Geolocation service failed.");
            initialLocation = newyork;
        } else {
            alert("Your browser doesn't support geolocation. We've placed you in Siberia.");
            initialLocation = siberia;
        }
        map.setCenter(initialLocation);
    }
    google.maps.event.addListener(autocomplete, 'place_changed', function () { resetCenter(); });
}
google.maps.event.addDomListener(window, 'load', initialize);
function getRecommendation(type, r, c) {
    $.get(callbackUrl + "?query=" + type + "&latitude=" + initialLocation.k + "&longitude=" + initialLocation.D + "&radius=" + r + "&category=" + c, recommendationCallback);
}
function recommendationCallback(data, status) {
    if (status == 'success') {
        clearMarkers();
        $("#recommendationList").empty()
        recommendation = JSON.parse(data);
        if (recommendation.length > 0) {
            $("#recommendation").removeClass("hidden");
        }
        else {
            $("#recommendation").addClass("hidden");
        }
        for (var i in recommendation) {
            $("#recommendationList").append("<li>");
            $("#recommendationList").append("<table><tr class='iw_table_row'><td>" + recommendation[i].NAME + "</td><tr>");
            $("#recommendationList").append("<tr class='iw_table_row'><td>" + recommendation[i].ADDRESS_LINE + "," + recommendation[i].CITY + "</td><tr>");
            $("#recommendationList").append("<tr class='iw_table_row'><td>" + recommendation[i].PHONE + "</td><tr>");

            if (recommendation[i].PRICE_LEVEL) {
                var priceHtml = '';
                for (var j = 0; j < 5; j++) {
                    if (recommendation[i].PRICE_LEVEL > j) {
                        priceHtml += '&#36;';
                    }
                }
                $("#recommendationList").append("<tr class='iw_table_row'><td>" + priceHtml + "</td><tr>");
            }
            if (recommendation[i].RATING) {
                var ratingHtml = '';
                for (var j = 0; j < 5; j++) {
                    if (recommendation[i].RATING < (j + 0.5)) {
                        ratingHtml += '&#10025;';
                    } else {
                        ratingHtml += '&#10029;';
                    }
                }
                $("#recommendationList").append("<tr class='iw_table_row'><td>" + ratingHtml + "</td><tr>");
            }
            $("#recommendationList").append("</table></li>");
            markers[i] = new google.maps.Marker({
                position: new google.maps.LatLng(recommendation[i].LATITUDE, recommendation[i].LONGITUDE),
                animation: google.maps.Animation.DROP,
                icon: g_markerIcon
            });
            markers[i].placeResult= recommendation[i].PLACE_ID;
            google.maps.event.addListener(markers[i], 'click', showInfoWindow1);
            setTimeout(dropMarker(i), i * 100);
        }
    }

}
function showInfoWindow1() {
    var marker = this;
    places.getDetails({ placeId: marker.placeResult },
        function (place, status) {
            if (status != google.maps.places.PlacesServiceStatus.OK) {
                return;
            }
            infoWindow.open(map, marker);
            buildIWContent(place);
        });
}
function callback(results, status) {
    if (status == google.maps.places.PlacesServiceStatus.OK) {
        clearMarkers();
        for (var i = 0; i < results.length; i++) {
            markers[i] = new google.maps.Marker({
                position: results[i].geometry.location,
                animation: google.maps.Animation.DROP,
                icon: g_markerIcon
            });
            markers[i].placeResult = results[i];
            google.maps.event.addListener(markers[i], 'click', showInfoWindow);
            setTimeout(dropMarker(i), i * 100);
            //setTimeout(senddata(i), i * 30000);
        }
    }
}
function senddata(i) {
    return function () {
        places.getDetails({ placeId: markers[i].placeResult.place_id }, function (data, status) {
            if (status == google.maps.places.PlacesServiceStatus.OK) {
                var m_data = JSON.stringify(data);
                $.ajax({
                    type: "POST",
                    url: callbackUrl + "?query=savedata",
                    data: m_data,
                    cache: false,
                    dataType: "json",
                    success: function (result) {

                    },
                    error: function (e) {
                    }
                });
            }
        });
    }
}
function getdata(r) {
    $("#recommendation").addClass("hidden");
    var request = {
        location: initialLocation,
        radius: r,
        types: [$("#drop_category option:selected").text().toLowerCase()]
    };
    places.nearbySearch(request, callback);
}
function clearMarkers() {
    for (var i = 0; i < markers.length; i++) {
        if (markers[i]) {
            markers[i].setMap(null);
        }
    }
    markers = [];
}
function dropMarker(i) {
    return function () {
        markers[i].setMap(map);
    };
}

function showInfoWindow() {
    var marker = this;
    places.getDetails({ placeId: marker.placeResult.place_id },
        function (place, status) {
            if (status != google.maps.places.PlacesServiceStatus.OK) {
                return;
            }
            infoWindow.open(map, marker);
            buildIWContent(place);
        });
}

function buildIWContent(place) {
    $('#placeID').val(place.place_id);
    $('#info-content').show();
    document.getElementById('iw-icon').innerHTML = '<img class="storeIcon" ' +
        'src="' + place.icon + '"/>';
    document.getElementById('iw-url').innerHTML = '<b><a href="' + place.url +
        '">' + place.name + '</a></b>';
    document.getElementById('iw-address').textContent = place.vicinity;

    if (place.formatted_phone_number) {
        document.getElementById('iw-phone-row').style.display = '';
        document.getElementById('iw-phone').textContent =
            place.formatted_phone_number;
    } else {
        document.getElementById('iw-phone-row').style.display = 'none';
    }

    // Assign a five-star rating to the hotel, using a black star ('&#10029;')
    // to indicate the rating the hotel has earned, and a white star ('&#10025;')
    // for the rating points not achieved.
    if (place.rating) {
        var ratingHtml = '';
        for (var i = 0; i < 5; i++) {
            if (place.rating < (i + 0.5)) {
                ratingHtml += '&#10025;';
            } else {
                ratingHtml += '&#10029;';
            }
            document.getElementById('iw-rating-row').style.display = '';
            document.getElementById('iw-rating').innerHTML = ratingHtml;
        }
    } else {
        document.getElementById('iw-rating-row').style.display = 'none';
    }

    // The regexp isolates the first part of the URL (domain plus subdomain)
    // to give a short URL for displaying in the info window.
    if (place.website) {
        var fullUrl = place.website;
        var website = hostnameRegexp.exec(place.website);
        if (website == null) {
            website = 'http://' + place.website + '/';
            fullUrl = website;
        }
        document.getElementById('iw-website-row').style.display = '';
        document.getElementById('iw-website').innerHTML = "<a href='" + website + "'>" + website + "</a>";
    } else {
        document.getElementById('iw-website-row').style.display = 'none';
    }
}
function resetCenter() {
    currentMarker.setVisible(false);
    var place = autocomplete.getPlace();
    if (!place.geometry) {
        return;
    }
    if (place.geometry.viewport) {
        initialLocation = place.geometry.location;
        map.fitBounds(place.geometry.viewport);
    } else {
        initialLocation = place.geometry.location;
        map.setCenter(initialLocation);
        map.setZoom(13);
    }
    currentMarker.setPosition(place.geometry.location);
    currentMarker.setVisible(true);
}
//
