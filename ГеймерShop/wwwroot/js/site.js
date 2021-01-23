function AddInCart(id) {
    $.get('/MyCart/Add', { Id: id }, function (data) {
        $('#modal-info').html(data);
        $('#smallModal').modal({
            show: true
        });
    });
}

function search() {
    var searchInput = $('#searachString').val();

    $('#products').load('/Games/Search', { searchName: searchInput });
}

function filter() {
    $('#products').load('/Games/Filter', {
        Genre: $('#Genres').val(),
        PlaingField: $('#PlaingFields').val(),
        Price: $('#Price').val()
    });
}