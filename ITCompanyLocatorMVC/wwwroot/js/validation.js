function validateCity() {
    var location = document.forms["searchForm"]["cityName"].value;
    var format = /^[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]*$/;

    if (location.match(format)) {
        alert("Special characters are not allowed..!!!");
        return false;
    }
}