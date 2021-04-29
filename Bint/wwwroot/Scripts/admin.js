$(document).ready(function () {


const Toast = Swal.mixin({
	toast: true,
	position: 'top-end',
	showConfirmButton: false,
	timer: 3000
});


$('#i1 ul li a').each(function (k, v) {
	if ($(this).attr('href') == "@b") {
		$(this).addClass("active");
	}
    });

});