import { Component, Input} from '@angular/core';
import {ControlValueAccessor} from '@angular/forms';

@Component({
  selector: 'select-input',
  templateUrl: './select-input.component.html',
  styleUrls: ['./select-input.component.css']
})
export class SelectInputComponent implements ControlValueAccessor 
{
  @Input() label: string;
  @Input() name: string;
  @Input() options: {value: string;}[];
  public selectedItem: string;
  onChanged: any = () => {};
  onTouched: any = () => {};
  
  constructor() { }
  
  optionSelected(event: any)
  {
    this.onChanged(this.selectedItem = event.target.value);
    console.log(this.selectedItem);
  }
  writeValue(obj: any): void
  {
    this.selectedItem = obj;
    
  }
  registerOnChange(fn: any): void
  {
    this.onChanged = fn;
    
  }
  registerOnTouched(fn: any): void
  {
    this.onTouched = fn;
    
  }  

}
