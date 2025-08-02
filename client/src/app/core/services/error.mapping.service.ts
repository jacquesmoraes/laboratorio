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
  
  mapServiceOrderError(error: ServiceOrderError | HttpErrorResponse | string): string {
    if (typeof error === 'string') {
      return error;
    }

    // Extrair mensagem do erro
    const errorMessage = this.extractErrorMessage(error);

    if (errorMessage.includes('Invalid operation: A ordem de serviço não possui estágios')) {
      return 'A ordem de serviço não possui estágios.';
    }
    
    if (errorMessage.includes('Invalid operation: Não há estágio aberto para enviar para prova')) {
      return 'Não há estágio aberto para enviar para prova.';
    }
    
    if (errorMessage.includes('Invalid operation: A ordem já está finalizada')) {
      return 'Não é possível alterar uma ordem finalizada.';
    }

    // Validações de ServiceOrderDateValidator
    if (errorMessage.includes('must occur after') || errorMessage.includes('after the last recorded stage')) {
      return 'A nova data deve ser posterior ao último estágio registrado.';
    }
    
    if (errorMessage.includes('Cannot send to try-in without prior stages')) {
      return 'Não é possível enviar para prova sem estágios anteriores.';
    }
    
    if (errorMessage.includes('Try-in date must be after the last recorded stage')) {
      return 'A data de prova deve ser posterior ao último estágio registrado.';
    }
    
    if (errorMessage.includes('Finish date cannot be earlier than the entry date')) {
      return 'A data de finalização não pode ser anterior à data de entrada.';
    }
    
    if (errorMessage.includes('Finish date must be after the last stage out date')) {
      return 'A data de finalização deve ser posterior à data de saída do último estágio.';
    }

    // Validações de ServiceOrderService
    if (errorMessage.includes('Client not found')) {
      return 'Cliente não encontrado.';
    }
    
    if (errorMessage.includes('Initial sector not found') || errorMessage.includes('Sector not found')) {
      return 'Setor não encontrado.';
    }
    
    if (errorMessage.includes('Service Order not found')) {
      return 'Ordem de serviço não encontrada.';
    }
    
    if (errorMessage.includes('Editing a finished service order is not allowed')) {
      return 'Não é permitido editar uma ordem finalizada.';
    }
    
    if (errorMessage.includes('Specified client does not exist')) {
      return 'O cliente especificado não existe.';
    }
    
    if (errorMessage.includes('Invalid service order ID')) {
      return 'ID da ordem de serviço inválido.';
    }
    
    if (errorMessage.includes('Deleting finished service orders is not allowed')) {
      return 'Não é permitido excluir ordens finalizadas.';
    }

    // Validações de Business Rules
    if (errorMessage.includes('Cannot schedule a finished service order')) {
      return 'Não é possível agendar uma ordem finalizada.';
    }
    
    if (errorMessage.includes('Cannot schedule a service order that is in try-in stage')) {
      return 'Não é possível agendar uma ordem que está em prova.';
    }
    
    if (errorMessage.includes('An active schedule already exists for this service order')) {
      return 'Já existe um agendamento ativo para esta ordem de serviço.';
    }
    
    if (errorMessage.includes('All service orders must belong to the same client')) {
      return 'Todas as ordens devem pertencer ao mesmo cliente.';
    }
    
    if (errorMessage.includes('No valid service order found')) {
      return 'Ordem de serviço ja finalizada.';
    }

    // Validações de ScheduleService
    if (errorMessage.includes('Cannot update a schedule that has already been delivered')) {
      return 'Não é possível atualizar um agendamento que já foi entregue.';
    }
    
    if (errorMessage.includes('Cannot remove a schedule that has already been delivered')) {
      return 'Não é possível remover um agendamento que já foi entregue.';
    }
    
    if (errorMessage.includes('For Sector Transfer scheduling, the destination sector must be provided')) {
      return 'Para transferência de setor, o setor de destino deve ser informado.';
    }
    
    if (errorMessage.includes('Destination sector must be different from the current sector')) {
      return 'O setor de destino deve ser diferente do setor atual.';
    }

    // Validações de CustomValidationException
    if (errorMessage.includes('Invalid sector ID')) {
      return 'ID do setor inválido.';
    }
    
    if (errorMessage.includes('Sector name is required')) {
      return 'Nome do setor é obrigatório.';
    }
    
    if (errorMessage.includes('Invalid client ID')) {
      return 'ID do cliente inválido.';
    }
    
    if (errorMessage.includes('Client name is required')) {
      return 'Nome do cliente é obrigatório.';
    }

    // Validações de BusinessRuleException
    if (errorMessage.includes('Cannot change billing mode while there are open service orders')) {
      return 'Não é possível alterar o modo de cobrança enquanto há ordens de serviço abertas.';
    }
    
    if (errorMessage.includes('A sector with the name')) {
      return 'Já existe um setor com este nome.';
    }
    
    if (errorMessage.includes('Failed to update sector')) {
      return 'Falha ao atualizar setor.';
    }
    
    if (errorMessage.includes('Failed to update client')) {
      return 'Falha ao atualizar cliente.';
    }

    // Validações legadas (mantidas para compatibilidade)
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

    // Se não encontrou mapeamento específico, retorna a mensagem original
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