// Write your JavaScript code.

var mainCheckbox = $("#mainCheckbox");
var itemCheckboxes = $(".itemCheckboxes");
var userLogin = $(".userLogin");
var userStatus = $(".userStatus");

var lockButton = $("#lockButton");
var unlockButton = $("#unlockButton");
var deleteButton = $("#deleteButton");

mainCheckbox.click(function () {
    var checked = 0;
    for (var i = 0; i < itemCheckboxes.length; i++) {
        if (!itemCheckboxes.eq(i).prop("checked")) {
            itemCheckboxes.eq(i).prop("checked", mainCheckbox.prop("checked"));
        }
        else {
            checked++;
        }
        if (checked == itemCheckboxes.length) {
            for (var j = 0; j < itemCheckboxes.length; j++) {
                itemCheckboxes.eq(j).prop("checked", mainCheckbox.prop("checked"));
            }
        }
        checkChoice();
    }
});

itemCheckboxes.click(function () {
    checkChoice();
});

function checkChoice() {
    var countOfChecked = 0;
    var countOfBlock = 0;
    var countOfActive = 0;
    var prevStatus = "";
    for (var i = 0; i < itemCheckboxes.length; i++) {
        if (itemCheckboxes.eq(i).prop("checked")) {
            if (userStatus.eq(i).text() == "Active")
                countOfActive++;
            else
                countOfBlock++;
            countOfChecked++;
        }
    }
    countOfChecked > 0 ? deleteButton.prop("disabled", false) : deleteButton.prop("disabled", true); 
    if ((countOfActive == 0) && (countOfBlock > 0)) {
        unlockButton.prop("disabled", false);
        lockButton.prop("disabled", true);
    }
    if ((countOfActive > 0) && (countOfBlock == 0)) {
        lockButton.prop("disabled", false);
        unlockButton.prop("disabled", true);
    }
    if ((countOfActive == 0) && (countOfBlock == 0) || (countOfActive > 0) && (countOfBlock > 0)) {
        lockButton.prop("disabled", true);
        unlockButton.prop("disabled", true);
    }
}

function getCheckedLogins() {
    var returnLogins = "";
    for (var i = 0; i < itemCheckboxes.length; i++) {
        if (itemCheckboxes.eq(i).prop("checked")) {
            if (returnLogins == "")
                returnLogins += userLogin.eq(i).text();
            else
                returnLogins += "," + userLogin.eq(i).text();
        }
    }
    return returnLogins;
}

deleteButton.click(function () {
    var returnLogins = getCheckedLogins();
    $.post("https://localhost:44349/Account/DeleteUsers", { 'returnLogins': returnLogins }, function (data) {
        $(location).attr('href', "https://localhost:44349");
    });
});

lockButton.click(function () {
    var returnLogins = getCheckedLogins();
    $.post("https://localhost:44349/Account/LockUsers", { 'returnLogins': returnLogins }, function (data) {
        $(location).attr('href', "https://localhost:44349");
    });
});

unlockButton.click(function () {
    var returnLogins = getCheckedLogins();
    $.post("https://localhost:44349/Account/UnlockUsers", { 'returnLogins': returnLogins }, function (data) {
        $(location).attr('href', "https://localhost:44349");
    });
});

