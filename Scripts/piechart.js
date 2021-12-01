window.onload = function () {
    const stuff = document.querySelectorAll('#data-stuff');
    const string = document.querySelectorAll('#data-string');
    const colorX = document.querySelectorAll('#data-color');

    var arrColor = [];
    var arrStuff = [];
    var arrString = [];
    var background = '';
    var persent = 0;


    function getRandomColor() {
        var letters = '0123456789ABCDEF';
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    for (var i = 0; i < stuff.length; i++) {
        arrColor.push(getRandomColor());
    }

    stuff.forEach((item) => {
        arrStuff.push(item.textContent);
    });
    string.forEach((item) => {
        arrString.push(item.textContent);
    });
    for (var i = 0; i < colorX.length; i++) {
        colorX[i].style.color = arrColor[i];
    }
    

    background = 'radial-gradient(circle closest-side, transparent 51%, white 0), conic-gradient( ';

    for (var i = 0; i < colorX.length; i++) {

        persent += parseInt(arrStuff[i]);
        if (persent < 100) {
            background += arrColor[i] + ' ' + '0%  , ' + arrColor[i] + ' ' + persent + '% , ';
        } else {
            background += arrColor[i] + ' ' + '0% , ' + arrColor[i] + ' ' + persent + '% ';
        }
    }

    background += ')';

    console.log(background);

    $(".pie-chart").css('background', background);
};