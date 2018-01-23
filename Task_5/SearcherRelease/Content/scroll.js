var page = 1;
var mainPage = $(".container");
var spiner = $("#spin");
var upButton = $("#upButton");
var searchButton = $("#searchButton");
var flagOfEnd = false;

searchButton.click(function () {
    flagOfEnd = false;
});

upButton.click(function () {
    window.scroll(0, 0);
});

$(document).scroll(function (event) {
    if ($(window).scrollTop() > 250) upButton.css('display', 'block');
    else upButton.css('display', 'none');
    if (window.$(window).scrollTop() >= window.$(document).height() - window.$(window).height() - 1) {
        if (!flagOfEnd)
            getItems();
    }
});

function getItems() {
    $.ajax({
        url: "/Home/GetNextPage",
        data: { page: page },
        beforeSend: function () {
            spiner.css('visibility','visible');
        },
        success: function (data) {
            if (data == -1) flagOfEnd = true;
            page++;
            console.log("Page " + page);
            console.log(data.length);
            for (var i = 0; i < data.length; i++) {
                var count = data[i].Count == "" ? "Нет в наличии" : "Количество: " + data[i].Count + " шт.";
                mainPage.append(`<a href="${data[i].Href}" target="_blank">
                    <div class="col-sm-6 col-md-4 itemConteiner">
                        <div class="thumbnail">
                            <img class="itemPic" src="${data[i].Image}" alt="${data[i].Name}">
                            <div class="caption">
                                <h4 class="itemName">${data[i].Name}</h4>
                                <p>${count}</p>
                                <p>Цена: ${data[i].Cost}</p>
                            </div>
                        </div>
                    </div>
                </a>`);
            }
            spiner.css('visibility', 'hidden');
        },
        dataType: 'json',
        error: function () {
            alert("Error while retrieving data!");
        }
    });
}