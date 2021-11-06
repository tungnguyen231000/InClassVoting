$(document).ready(function () {
    console.log('More');


    $('.btn-more').click(function () {
        $('.more').before('<tr> <td class="w-2"><input type="checkbox" id="cb-option2" name="cb-option" ></td> <td><input type="text" class="w-full pl-3" name="option" placeholder="Option More"></td></tr>');
    });

    
    $('.btn-fillblank').click(function () {
        console.log('More-FillBlank');
        $('.more-fillblank').before('<tr> <td><input type="text" class="w-full pl-3" name="answer" placeholder="Answer More"></td></tr >');
    });

    $('.btn-matching').click(function () {
        console.log('More-Matching');
        $('.more-matching').before('<tr> <td> <input type="type" class="text-center" name="txt-left" value="" placeholder="Right" /></td><td>-</td><td><input type="type" class="text-center" name="txt-right" value="" placeholder="Left" /></td></tr >');
    });
});