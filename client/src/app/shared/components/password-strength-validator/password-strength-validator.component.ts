import { Component, input, signal, computed, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { PasswordValidationService, PasswordRequirement } from '../../../core/services/password-validation.service';

@Component({
  selector: 'app-password-strength-validator',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="password-validator">
      <!-- Barra de força compacta -->
      <div class="strength-indicator">
        <div class="strength-dots">
          <div class="dot" [class]="getDotClass(1)"></div>
          <div class="dot" [class]="getDotClass(2)"></div>
          <div class="dot" [class]="getDotClass(3)"></div>
        </div>
        <span class="strength-label">{{ getStrengthText() }}</span>
      </div>

      <!-- Requisitos compactos (só aparecem quando há erro) -->
      <div *ngIf="hasPassword() && !validationResult()?.isValid" class="requirements-compact">
        <div class="requirement-error" *ngFor="let requirement of getInvalidRequirements()">
          <mat-icon class="error-icon">error</mat-icon>
          <span>{{ requirement.label }}</span>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./password-strength-validator.component.scss']
})
export class PasswordStrengthValidatorComponent {
  private passwordValidationService = inject(PasswordValidationService);

  // Input para receber a senha do componente pai
  password = input.required<string>();

  // Computed signals para validação
  validationResult = computed(() =>
    this.passwordValidationService.validatePassword(this.password())
  );

  requirements = computed(() =>
    this.passwordValidationService.getRequirements()
  );

  // Métodos auxiliares
  isRequirementValid(requirement: PasswordRequirement): boolean {
    return this.passwordValidationService.validateRequirement(requirement.id, this.password());
  }

  // Versão compacta (pontinhos)
  getDotClass(position: number): string {
    if (!this.hasPassword()) return 'empty';

    const strength = this.validationResult()?.strength;
    switch (strength) {
      case 'weak': return position <= 1 ? 'weak' : 'empty';
      case 'medium': return position <= 2 ? 'medium' : 'empty';
      case 'strong': return 'strong';
      default: return 'empty';
    }
  }

  getInvalidRequirements(): PasswordRequirement[] {
    return this.requirements().filter(req => !this.isRequirementValid(req));
  }

  getStrengthText(): string {
    if (!this.hasPassword()) return '';

    const strength = this.validationResult()?.strength;
    switch (strength) {
      case 'weak': return 'Fraca';
      case 'medium': return 'Média';
      case 'strong': return 'Forte';
      default: return '';
    }
  }

  getStrengthClass(): string {
    return this.validationResult()?.strength || '';
  }

  getRequirementClass(requirement: PasswordRequirement): string {
    return this.isRequirementValid(requirement) ? 'valid' : 'invalid';
  }

  getRequirementIcon(requirement: PasswordRequirement): string {
    return this.isRequirementValid(requirement) ? 'check_circle' : 'cancel';
  }

  getRequirementIconClass(requirement: PasswordRequirement): string {
    return this.isRequirementValid(requirement) ? 'text-success' : 'text-danger';
  }

  hasPassword(): boolean {
    return this.password() !== undefined && this.password() !== null && this.password().length > 0;
  }

  // Getter para verificar se a senha é válida (útil para o componente pai)
  get isValid(): boolean {
    return this.validationResult()?.isValid ?? false;
  }
}
