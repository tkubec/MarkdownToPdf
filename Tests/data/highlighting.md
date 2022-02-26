
# Block code

## With highlighting

```python
# Example

num = int(input("Enter a number: "))    # user input
factorial = 1    
if num < 0:    
    print(" Factorial does not exist for negative numbers")    
elif num == 0:    
    print("The factorial of 0 is 1")    
else:    
    for i in range(1, num + 1):    
        factorial = factorial * i    
    print("The factorial of", num, "is", factorial)    
```


## Without highlighting

```unknown
# Example

num = int(input("Enter a number: "))    # user input
factorial = 1    
if num < 0:    
    print(" Factorial does not exist for negative numbers")    
elif num == 0:    
    print("The factorial of 0 is 1")    
else:    
    for i in range(1, num + 1):    
        factorial = factorial * i    
    print("The factorial of", num, "is", factorial)    
```

# Inline

This is a code snippet  `range(1, num + 1)` within a text block.

This is a highlighted code snippet  `range(1, num + 1)`{.python} within a text block.
