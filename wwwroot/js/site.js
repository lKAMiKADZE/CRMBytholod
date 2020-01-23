// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// отключение кнопки формы
//$(document).ready(function () {
//    $("#form").keydown(function (event) {
//        if (event.keyCode == 13) {
//            event.preventDefault();
//            return false;
//        }
//    })
//});


// получение улицы при создании изменении заявки filtrOrders_Adres
$(function () {
    $("#order_STREET").typeahead({
        hint: true,
        highlight: true,
        minLength: 1,
        source: function (request, response) {
            $.ajax({
                url: '/Home/AutoComplete?prefix=' + request,
                //data: "{ 'prefix': '" + request + "'}",
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    items = [];
                    map = {};
                    $.each(data, function (i, item) {
                        var id = item.iD_KLADR;
                        var name = item.nameStreet;
                        map[name] = { id: id, name: name };
                        items.push(name);
                    });
                    response(items);
                    $(".dropdown-menu").css("height", "auto");
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        updater: function (item) {
            $('#hfCustomer').val(map[item].id);
            return item;
        }
    });    
});



// получение улицы при поиске заявки 
$(function () {
    $("#filtrOrders_Adres").typeahead({
        hint: true,
        highlight: true,
        minLength: 1,
        source: function (request, response) {
            $.ajax({
                url: '/Home/AutoComplete?prefix=' + request,
                //data: "{ 'prefix': '" + request + "'}",
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    items = [];
                    map = {};
                    $.each(data, function (i, item) {
                        var id = item.iD_KLADR;
                        var name = item.nameStreet;
                        map[name] = { id: id, name: name };
                        items.push(name);
                    });
                    response(items);
                    $(".dropdown-menu").css("height", "auto");
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        updater: function (item) {
            $('#hfCustomer').val(map[item].id);
            return item;
        }
    });
});

// для маски телефона
$(function () {
    $('[mask]').each(function (e) {
        $(this).mask($(this).attr('mask'));
    });
});