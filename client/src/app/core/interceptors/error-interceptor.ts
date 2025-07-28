import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

interface ApiErrorResponse {
  message: string;
  statusCode: number;
  errors?: string[];
  error?: string;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado.';

      if (error.error) {
        const apiError = error.error as ApiErrorResponse;
        
        // Extrair mensagem específica do backend
        if (apiError.message) {
          errorMessage = apiError.message;
        } else if (apiError.error) {
          errorMessage = apiError.error;
        } else if (apiError.errors && apiError.errors.length > 0) {
          errorMessage = apiError.errors[0];
        }
      }

      // Mostrar erro apenas para erros de validação (400) e regras de negócio (422)
      if (error.status === 400 || error.status === 422) {
        snackBar.open(errorMessage, 'Fechar', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }

      return throwError(() => error);
    })
  );
};