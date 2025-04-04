function MensagemSucesso(msg, func1, func2) {
    MsgAlerta(msg, "S", func1, func2);
}

function MensagemErro(msg, func1, func2) {
    MsgAlerta(msg, "E", func1, func2);
}

function MensagemAlerta(msg, func1, func2) {
    MsgAlerta(msg, "A", func1, func2);
}

function MensagemConfirmacao(msg, func1, func2) {
    MsgAlerta(msg, "C", func1, func2);
}

function MsgAlerta(msg, tipo, func1, func2) {

    if (tipo == 'S') {
        $('body').append('<div class="modal fade" data-backdrop="static" data-keyboard="false" id="response"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">Sucesso</h4></div><div class="modal-body">' + msg + '</div><div class="modal-footer"><button type="button" class="btn btn-info btn-ok" data-dismiss="modal">Ok</button></div></div></div></div><div class="modal-backdrop fade in"></div>');
    }
    else if (tipo == 'E') {
        $('body').append('<div class="modal fade" data-backdrop="static" data-keyboard="false" id="response"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">Erro</h4></div><div class="modal-body">' + msg + '</div><div class="modal-footer"><button type="button" class="btn btn-danger btn-ok" data-dismiss="modal">Ok</button></div></div></div></div><div class="modal-backdrop fade in"></div>');
    }
    else if (tipo == 'C') {
        $('body').append('<div class="modal fade" data-backdrop="static" data-keyboard="false" id="response"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">Confirmação</h4></div><div class="modal-body">' + msg + '</div><div class="modal-footer"><button type="button" class="btn btn-white btn-nao" data-dismiss="modal">Não</button><button type="button" class="btn btn-info btn-sim">Sim</button></div></div></div></div><div class="modal-backdrop fade in"></div>');
    }
    else if (tipo == 'A') {
        $('body').append('<div class="modal fade" data-backdrop="static" data-keyboard="false" id="response"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">Aviso</h4></div><div class="modal-body">' + msg + '</div><div class="modal-footer"><button type="button" class="btn btn-white btn-ok" data-dismiss="modal">Ok</button></div></div></div></div><div class="modal-backdrop fade in"></div>');
    }

    $('#response').modal({
        show: true,
        backdrop: 'static'
    });

    $('#response').find(".btn-sim").focus();

    $('#response').find('.modal-dialog .btn-sim, .modal-dialog .btn-ok').click(function () {
        $('#response').remove();
        $('.modal-backdrop').remove();
        $('.modal-open').removeClass('modal-open').removeAttr('style');
        if (func1 != null) {
            func1();
        }
    });

    $('#response').find('.modal-dialog .btn-nao').click(function () {
        $('#response').remove();
        $('.modal-backdrop').remove();
        $('.modal-open').removeClass('modal-open').removeAttr('style');
        if (func2) {
            func2();
        }
    });

}