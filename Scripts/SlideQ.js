$(document).ready(function () {

    var countNext = 0;
    var listTime = document.querySelectorAll('#time');
    var listDot = '';

    var change = true;

    for (var x = 0; x < listTime.length; x++) {
        listDot += '<span class="dot"></span>';
    }
    $('#center-dot').html(listDot);

const carouselSlider = (function () {
    let _slideIndex = 1;

    const _slides = document.querySelectorAll(".image-fade");
    const _dots = document.querySelectorAll(".dot");

    function _sliderInitState(slides, dots) {
        for (let i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }

        for (let i = 0; i < dots.length; i++) {
            dots[i].classList.remove("active");
        }
    }

    function _checkSlideIndexBoundary(slideIndex, slides) {
        if (slideIndex > slides.length) {
            _slideIndex = 1;
        }
        if (slideIndex === 0) {
            _slideIndex = slides.length;
        }
    }

    function _slideSelectionIndecator(dots, slideIndex) {
        dots[slideIndex - 1].classList.add("active");
    }

    function _activateSlide(slides, slideIndex) {
        slides[slideIndex - 1].style.display = "block";
        _slideSelectionIndecator(_dots, _slideIndex);
    }

    function _imageDirection(direction) {
        _sliderInitState(_slides, _dots);
        if (direction == "next") {
            _activateSlide(_slides, _slideIndex);
            _slideIndex++;
            _checkSlideIndexBoundary(_slideIndex, _slides);
        } else {
            _slideIndex--;
            _checkSlideIndexBoundary(_slideIndex, _slides);
            _activateSlide(_slides, _slideIndex);
        }
    }

    const _previousButton = document.querySelector(".previous");
    const _nextButton = document.querySelector(".next");


    $(_previousButton).click(function () {
        _imageDirection("previous");

        
        if (change == false) {

            if (countNext == 0) {
                countNext = listTime.length - 1;
            } else {
                --countNext;
            }

            if (countNext >= (listTime.length - 1)) {
                $('#nextOrder').val("Submit");
            } else {
                $('#nextOrder').val("Next");
            }
            if (countNext < listTime.length) {
                console.log(listTime[countNext].getAttribute('data-time'));
                console.log(countNext);
                timeCountDown(countNext);
            }
            if (countNext == listTime.length) {
                $('#formOrder').submit();
                $('#nextOrder').prop('disabled', true);
                $('#nextOrder').off('click');
                $('#previousOrder').prop('disabled', true);
                $('#previousOrder').off('click');
            }
        } else {
            change = false;
        }

    });

    $(_nextButton).click(function () {
        _imageDirection("next");

        if (change == true) {
            ++countNext;


            if (countNext >= (listTime.length - 1)) {
                $('#nextOrder').val("Submit");
            } else {
                $('#nextOrder').val("Next");
            }
            if (countNext < listTime.length) {
                console.log(listTime[countNext].getAttribute('data-time'));
                console.log(countNext);
                timeCountDown(countNext);
            }
            if (countNext == listTime.length) {
                $('#formOrder').submit();
                $('#nextOrder').prop('disabled', true);
                $('#nextOrder').off('click');
                $('#previousOrder').prop('disabled', true);
                $('#previousOrder').off('click');
            }
        } else {
            change = true;
        }
        

    });


    const slide = function () {
        _sliderInitState(_slides, _dots);
        _activateSlide(_slides, _slideIndex);
        _slideIndex++;
        _checkSlideIndexBoundary(_slideIndex, _slides);
        // Change question every 20 seconds
    };

    return {
        slide
    };
})();

    carouselSlider.slide();
    timeCountDown(0);

    function timeCountDown(element) {
        var time = listTime[element];
        var value = time.getAttribute('data-time') * 1000;

        var countDownDate = new Date().getTime() + value;

        // Update the count down every 1 second
        var z = setInterval(function () {

            // Get today's date and time
            var now = new Date().getTime();

            // Find the distance between now and the count down date
            var distance = countDownDate - now;

            // Time calculations for days, hours, minutes and seconds
            var days = Math.floor(distance / (1000 * 60 * 60 * 24));
            var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)) + (days * 24);
            var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60)) + (hours * 60);
            var seconds = Math.floor((distance % (1000 * 60)) / 1000);

            // Display the result in the element with id="demo"
            time.innerHTML = format(minutes) + ":" + format(seconds);

            // If the count down is finished, write some text
            if (distance <= 0) {
                clearInterval(z);
                $('#nextOrder').click();
            }
        }, 1000);


        $('#nextOrder').mousedown(function () {
            clearInterval(z);
        });
        $('#previousOrder').mousedown(function () {
            clearInterval(z);
        });
    }

    function format(x) {
        if (x < 10) {
            return '0' + x;
        } else {
            return x
        }
    }

});