import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { BusyService } from '../../../core/services/busy.service';
@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIcon, MatButton, RouterLink],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss',
})
export class EmptyStateComponent {}
