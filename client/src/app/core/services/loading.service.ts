import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private requestCount = 0;
  private _isLoading = signal<boolean>(false);
  
  public readonly isLoading = this._isLoading.asReadonly();

  show(): void {
    this.requestCount++;
    this._isLoading.set(true);
  }

  hide(): void {
    this.requestCount--;
    if (this.requestCount <= 0) {
      this.requestCount = 0;
      this._isLoading.set(false);
    }
  }

  reset(): void {
    this.requestCount = 0;
    this._isLoading.set(false);
  }
}