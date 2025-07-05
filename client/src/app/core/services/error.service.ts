import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class ErrorService {

  showError(title: string, error?: unknown): void {
    console.error(title, error);
    Swal.fire({
      icon: 'error',
      title: 'Erro!',
      text: title,
      confirmButtonText: 'OK'
    });
  }

  showSuccess(message: string): void {
    Swal.fire({
      icon: 'success',
      title: 'Sucesso!',
      text: message,
      timer: 1000,
      showConfirmButton: false
    });
  }

  confirm(title: string, text: string) {
    return Swal.fire({
      title,
      text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim',
      cancelButtonText: 'Cancelar'
    });
  }
}
