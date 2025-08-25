import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

interface ApiErrorResponse {
  message?: string;
  error?: string;
  errors?: string[];
  statusCode?: number;
}

interface ServiceOrderError {
  error?: ApiErrorResponse;
  message?: string;
  status?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorMappingService {
  
  private readonly errorMessages: { [key: string]: string } = {
    // Erros de autenticação
    'Invalid credentials.': 'Email ou senha inválidos',
    'Invalid credentials': 'Email ou senha inválidos',
    'Account is deactivated.': 'Conta desativada. Entre em contato com o administrador.',
    'Account is not activated.': 'Conta não ativada. Entre em contato com o administrador.',
    'User not found.': 'Usuário não encontrado',
    'User is not in first access state.': 'Usuário não está em primeiro acesso',
    'The access code has expired. Request a new one from the administrator.': 'Código de acesso expirado. Solicite um novo código ao administrador.',
    'Invalid access code.': 'Código de acesso inválido',
    'Failed to reset password.': 'Erro ao redefinir senha',
    'This email is already in use.': 'Este email já está em uso',
    'Passwords do not match.': 'As senhas não coincidem',
    'Error while registering client user.': 'Erro ao registrar usuário cliente',
    'Error while registering admin user.': 'Erro ao registrar usuário administrador',
    'Client user not found or has already completed first access.': 'Cliente não encontrado ou já completou o primeiro acesso',
    'Failed to update access code.': 'Erro ao atualizar código de acesso',
    'Token inválido: clientId não encontrado.': 'Token inválido. Faça login novamente.',
    'Token inválido: clientId não é um número válido.': 'Token inválido. Faça login novamente.',
    'Token inválido: clientId deve ser maior que zero.': 'Token inválido. Faça login novamente.',

    // Erros de validação
    'The Email field is required.': 'Email é obrigatório',
    'The Password field is required.': 'Senha é obrigatória',
    'The DisplayName field is required.': 'Nome é obrigatório',
    'The UserName field is required.': 'Nome de usuário é obrigatório',
    'The ClientId field is required.': 'Cliente é obrigatório',
    'The AccessCode field is required.': 'Código de acesso é obrigatório',
    'The NewPassword field is required.': 'Nova senha é obrigatória',
    'The ConfirmNewPassword field is required.': 'Confirmação da nova senha é obrigatória',

    // Erros de negócio - Service Orders
    'Invalid operation: A ordem de serviço não possui estágios': 'A ordem de serviço não possui estágios.',
    'Invalid operation: Não há estágio aberto para enviar para prova': 'Não há estágio aberto para enviar para prova.',
    'Invalid operation: A ordem já está finalizada': 'Não é possível alterar uma ordem finalizada.',
    'Client not found': 'Cliente não encontrado.',
    'Initial sector not found': 'Setor não encontrado.',
    'Sector not found': 'Setor não encontrado.',
    'Service Order not found': 'Ordem de serviço não encontrada.',
    'Editing a finished service order is not allowed': 'Não é permitido editar uma ordem finalizada.',
    'Specified client does not exist': 'O cliente especificado não existe.',
    'Invalid service order ID': 'ID da ordem de serviço inválido.',
    'Deleting finished service orders is not allowed': 'Não é permitido excluir ordens finalizadas.',
    'Cannot schedule a finished service order': 'Não é possível agendar uma ordem finalizada.',
    'Cannot schedule a service order that is in try-in stage': 'Não é possível agendar uma ordem que está em prova.',
    'An active schedule already exists for this service order': 'Já existe um agendamento ativo para esta ordem de serviço.',
    'All service orders must belong to the same client': 'Todas as ordens devem pertencer ao mesmo cliente.',
    'No valid service order found': 'Ordem de serviço já finalizada.',

    // Validações de datas
    'Cannot send to try-in if there is no open stage.': 'Não é possível enviar para prova se não há estágio aberto.',
    'Cannot send to try-in without prior stages.': 'Não é possível enviar para prova sem estágios anteriores.',
    'The new stage must occur after the last recorded stage.': 'A nova data deve ser posterior ao último estágio registrado.',
    'must occur after': 'A nova data deve ser posterior ao último estágio registrado.',
    'after the last recorded stage': 'A nova data deve ser posterior ao último estágio registrado.',
    'Try-in date must be after the last recorded stage': 'A data de prova deve ser posterior ao último estágio registrado.',
    'Finish date cannot be earlier than the entry date': 'A data de finalização não pode ser anterior à data de entrada.',
    'Finish date must be after the last stage out date': 'A data de finalização deve ser posterior à data de saída do último estágio.',

    
    // Erros de agendamento
    'Cannot update a schedule that has already been delivered': 'Não é possível atualizar um agendamento que já foi entregue.',
    'Cannot remove a schedule that has already been delivered': 'Não é possível remover um agendamento que já foi entregue.',
    'For Sector Transfer scheduling, the destination sector must be provided': 'Para transferência de setor, o setor de destino deve ser informado.',
    'Destination sector must be different from the current sector': 'O setor de destino deve ser diferente do setor atual.',

    // Validações de CustomValidationException
    'Invalid sector ID': 'ID do setor inválido.',
    'Sector name is required': 'Nome do setor é obrigatório.',
    'Invalid client ID': 'ID do cliente inválido.',
    'Client name is required': 'Nome do cliente é obrigatório.',

    // Validações de BusinessRuleException
    'Cannot change billing mode while there are open service orders': 'Não é possível alterar o modo de cobrança enquanto há ordens de serviço abertas.',
    'A sector with the name': 'Já existe um setor com este nome.',
    'Failed to update sector': 'Falha ao atualizar setor.',
    'Failed to update client': 'Falha ao atualizar cliente.',

    // Erros de outros recursos
    'Invoice not found': 'Fatura não encontrada',
    'Payment not found': 'Pagamento não encontrado',
    'Work section not found': 'Seção de trabalho não encontrada',
    'Work type not found': 'Tipo de trabalho não encontrado',
    'Scale not found': 'Escala não encontrada',
    'Shade not found': 'Cor não encontrada',
    'Table price not found': 'Tabela de preço não encontrada',

    // Erros de permissão
    'Access denied': 'Acesso negado',
    'You do not have permission to perform this action': 'Você não tem permissão para realizar esta ação',
    'Only administrators can perform this action': 'Apenas administradores podem realizar esta ação',
    'Only clients can perform this action': 'Apenas clientes podem realizar esta ação',

    // Erros de rede/servidor
    'Network error': 'Erro de conexão. Verifique sua internet.',
    'Server error': 'Erro no servidor. Tente novamente mais tarde.',
    'Timeout error': 'Tempo limite excedido. Tente novamente.',
    'Connection refused': 'Conexão recusada. Verifique se o servidor está online.',

    // Erros genéricos
    'An error occurred while processing your request': 'Ocorreu um erro ao processar sua solicitação',
    'Something went wrong': 'Algo deu errado',
    'Please try again': 'Tente novamente',
    'Internal server error': 'Erro interno do servidor',
    'Bad request': 'Requisição inválida',
    'Not found': 'Recurso não encontrado',
    'Unauthorized': 'Não autorizado',
    'Forbidden': 'Acesso negado',
    // Erros de upload de arquivo
  'Nenhum arquivo foi enviado.': 'Nenhum arquivo foi selecionado.',
  'Tipo de arquivo não permitido. Use apenas: jpg, jpeg, png, gif, webp': 'Tipo de arquivo não permitido. Use apenas: jpg, jpeg, png, gif, webp',
  'Arquivo muito grande. Tamanho máximo: 5MB': 'Arquivo muito grande. Tamanho máximo: 5MB',
  'Erro interno do servidor ao fazer upload.': 'Erro interno do servidor ao fazer upload.',
  'Erro interno do servidor ao deletar arquivo.': 'Erro interno do servidor ao deletar arquivo.',
  'Nome do arquivo é obrigatório.': 'Nome do arquivo é obrigatório.',
  'Arquivo não encontrado.': 'Arquivo não encontrado.',
  'Upload failed': 'Falha no upload do arquivo.',
  };

  mapServiceOrderError(error: ServiceOrderError | HttpErrorResponse | string): string {
    if (typeof error === 'string') {
      return this.errorMessages[error] || error;
    }

    const errorMessage = this.extractErrorMessage(error);
    
    // Procura por mapeamento exato primeiro
    if (this.errorMessages[errorMessage]) {
      return this.errorMessages[errorMessage];
    }

    // Procura por mapeamento parcial
    for (const [key, value] of Object.entries(this.errorMessages)) {
      if (errorMessage.includes(key)) {
        return value;
      }
    }

    // Fallback para mensagens legadas
    if (errorMessage.includes('não há estágio aberto')) {
      return 'Não há estágio aberto para enviar para prova.';
    }
    
    if (errorMessage.includes('já está finalizada')) {
      return 'Não é possível alterar uma ordem finalizada.';
    }
    
    if (errorMessage.includes('já está em prova')) {
      return 'A ordem já está em prova.';
    }
    
    if (errorMessage.includes('data inválida')) {
      return 'Data inválida. Verifique a data informada.';
    }
    
    if (errorMessage.includes('same client')) {
      return 'Todas as ordens devem ser do mesmo cliente.';
    }

    return errorMessage || 'Ocorreu um erro inesperado.';
  }

  // Método específico para erros de autenticação
  getAuthErrorMessage(error: any): string {
    const errorMessage = this.extractErrorMessage(error);
    
    // Verifica se errorMessage é uma string válida
    if (typeof errorMessage !== 'string') {
      return 'Ocorreu um erro inesperado.';
    }
    
    // Procura por mapeamento específico de auth
    if (this.errorMessages[errorMessage]) {
      return this.errorMessages[errorMessage];
    }
  
    // Casos específicos de autenticação
    if (errorMessage.includes('Invalid credentials') || errorMessage.includes('Email ou senha inválidos')) {
      return 'Email ou senha inválidos';
    }
    
    if (errorMessage.includes('access code') || errorMessage.includes('código de acesso')) {
      return 'Código de acesso inválido ou expirado. Verifique o código recebido por email.';
    }
    
    if (errorMessage.includes('first access') || errorMessage.includes('primeiro acesso')) {
      return 'Erro no primeiro acesso. Verifique suas credenciais e código de acesso.';
    }
    
    // Casos específicos de mudança de senha
    if (errorMessage.includes('Incorrect password') || errorMessage.includes('senha incorreta')) {
      return 'Senha atual incorreta. Verifique a senha digitada.';
    }
    
    if (errorMessage.includes('current password') || errorMessage.includes('senha atual')) {
      return 'Senha atual incorreta.';
    }
    
    if (errorMessage.includes('password') || errorMessage.includes('senha')) {
      return 'Erro ao alterar senha. Verifique os dados informados.';
    }
    
    return errorMessage || 'Ocorreu um erro inesperado.';
  }
  

  private extractErrorMessage(error: ServiceOrderError | HttpErrorResponse): string {
    // Se é HttpErrorResponse
    if (error instanceof HttpErrorResponse) {
      if (error.error) {
        const apiError = error.error as ApiErrorResponse;
        return apiError.message || apiError.error || '';
      }
      return error.message || '';
    }

    // Se é ServiceOrderError
    if (error.error) {
      const apiError = error.error as ApiErrorResponse;
      return apiError.message || apiError.error || '';
    }

    return error.message || '';
  }
}