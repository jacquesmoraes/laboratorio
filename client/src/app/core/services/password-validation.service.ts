import { Injectable } from '@angular/core';

export interface PasswordRequirement {
  id: string;
  label: string;
  validator: (password: string) => boolean;
  icon: string;
}

export interface PasswordValidationResult {
  isValid: boolean;
  errors: string[];
  requirements: PasswordRequirement[];
  strength: 'weak' | 'medium' | 'strong';
  strengthScore: number;
  validRequirements: PasswordRequirement[];
  invalidRequirements: PasswordRequirement[];
}

@Injectable({ providedIn: 'root' })
export class PasswordValidationService {
  // Lista de requisitos de senha
  private readonly requirements: PasswordRequirement[] = [
    {
      id: 'length',
      label: 'Mínimo 6 caracteres',
      validator: (pwd: string) => pwd.length >= 6,
      icon: 'check'
    },
    {
      id: 'digit',
      label: 'Pelo menos 1 número (0-9)',
      validator: (pwd: string) => /\d/.test(pwd),
      icon: 'check'
    },
    {
      id: 'lowercase',
      label: 'Pelo menos 1 letra minúscula (a-z)',
      validator: (pwd: string) => /[a-z]/.test(pwd),
      icon: 'check'
    },
    {
      id: 'uppercase',
      label: 'Pelo menos 1 letra maiúscula (A-Z)',
      validator: (pwd: string) => /[A-Z]/.test(pwd),
      icon: 'check'
    },
    {
      id: 'special',
      label: 'Pelo menos 1 caractere especial (!@#$%^&*)',
      validator: (pwd: string) => /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(pwd),
      icon: 'check'
    }
  ];

  validatePassword(password: string): PasswordValidationResult {
    const errors: string[] = [];
    const validRequirements: PasswordRequirement[] = [];
    const invalidRequirements: PasswordRequirement[] = [];

    // Validar cada requisito
    this.requirements.forEach(requirement => {
      if (requirement.validator(password)) {
        validRequirements.push(requirement);
      } else {
        invalidRequirements.push(requirement);
        errors.push(requirement.label);
      }
    });

    const strength = this.getPasswordStrength(password);
    const strengthScore = this.getPasswordStrengthScore(password);

    return {
      isValid: errors.length === 0,
      errors,
      requirements: this.requirements,
      strength,
      strengthScore,
      validRequirements,
      invalidRequirements
    };
  }

  getPasswordStrength(password: string): 'weak' | 'medium' | 'strong' {
    if (!password) return 'weak';

    const score = this.getPasswordStrengthScore(password);

    if (score <= 40) return 'weak';
    if (score <= 80) return 'medium';
    return 'strong';
  }

  private getPasswordStrengthScore(password: string): number {
    if (!password) return 0;

    let score = 0;

    // Comprimento
    if (password.length >= 6) score += 20;
    if (password.length >= 8) score += 10;
    if (password.length >= 10) score += 10;

    // Complexidade
    if (/[A-Z]/.test(password)) score += 20;
    if (/[a-z]/.test(password)) score += 20;
    if (/\d/.test(password)) score += 20;
    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) score += 20;

    return Math.min(score, 100);
  }

  // Método para obter apenas os requisitos (útil para o componente visual)
  getRequirements(): PasswordRequirement[] {
    return this.requirements;
  }

  // Método para validar um requisito específico
  validateRequirement(requirementId: string, password: string): boolean {
    const requirement = this.requirements.find(r => r.id === requirementId);
    return requirement ? requirement.validator(password) : false;
  }
}
