window.onload = function () {

    const numbs = document.querySelector("#number");
    const numb = document.querySelector(".numb");

    var persent = numbs.getAttribute("data-persent");

    let conturs =440 - 440 * (persent/100);
    let conter = 0;


    setInterval(() => {
        if (conturs == 0) {
            clearInterval();
        } else {
            const numbss = document.querySelector(".numb").style.strokeDashoffset = 'calc('+conturs+')';
            numb.textContent = numbss + "%";
        }
    }, 80);

    
    setInterval(() => {
        if (conter == persent) {
            clearInterval();
        } else {
            conter += 1;
            numbs.textContent = conter + "%";
        }
    }, 80 / ((persent / 100)));


};