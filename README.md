# Morpeh Entity Converter

Tools for serializing and converting gameobjects with hierarchy into entities for [Morpeh ECS](https://github.com/scellecs/morpeh).

> [!WARNING]
> This package is in the early stage of development for the Morpeh-2024, which has not yet been released. There will likely be many significant changes.

## Installation

Requirements:

- [morpeh #stage-2024.1](https://github.com/scellecs/morpeh/tree/stage-2024.1)
- [morpeh.transforms #stage-2024](https://github.com/heymeepo/morpeh.transforms/tree/stage-2024)

Dependencies:

- com.unity.burst: 1.8.12
- com.unity.collections: 2.1.4
- com.unity.serialization: 3.1.1

Install via git URL

```bash
https://github.com/heymeepo/morpeh.entity-converter.git
```

## Usage

> [!IMPORTANT]  
> When using this package, you must add ```WarmupComponentsTypesInitializer``` as the very first initializer in your list of systems.

In this approach, gameobjects are used as configurations for future entities. They are only needed in the editor, so they should have the tag "EditorOnly". ```ConvertToEntity``` sets this tag by default.

**```ConvertToEntity```** is a MonoBehaviour class that you should attach to the root GameObject/Prefab. You do not need to attach it to child GameObjects. 

**```EntityBakedDataAsset```**  is a ScriptableObject that is placed in ```ConvertToEntity```. It is a serialized representation of your entity hierarchy based on GameObject, and will be used at runtime.

**```EcsAuthoring```** is the primary MonoBehaviour component for converting gameobjects into entities. All user-declared authoring components must inherit it.

### Baking process
The rule is as follows: all game objects in the hierarchy that have any ```: EcsAuthoring``` component attached will be added to the baked entity hierarchy. All game objects in the hierarchy that do not have any authoring component will be ignored as if they were never there. 

I strongly advise against having such objects in the hierarchy because firstly, they serve no purpose, and secondly, you may get unexpected results since child entities inherit their local position relative to the parent gameobject, which could be a completely different object from what you expect in the editor.

All transform components such as ```LocalToWorld```, ```LocalTransform```, and ```PostTransformMatrix``` are automatically added during baking, you do not need to add them manually.

All operations related to adding data to the entity should occur inside the ```Bake()``` method of your ```: EcsAuthoring``` components, otherwise they will have no effect.

There are 2 options to add a component to an entity during data baking:

- ```SetComponent<T>(T data)``` 

Gather the necessary data from the prefab and pass an instance of your ```: IComponent``` struct to this method.

- ```SetComoponentDataUnsafe<T>(T data, int typeId)``` 

This is a somewhat dangerous way to add a component to an entity. Use it only if you are absolutely certain that you need to. 

The ```T data``` in this case can be any structure, either managed or unmanaged, but it must exactly match the memory layout of the component you want to write it into.

The ```int typeId``` is the runtime ID of the component, which can be obtained using the method ```GetComponentTypeId<T>()``` / ```GetComponentTypeId(Type componentType)```.

```int typeId``` is valid only during the baking process and cannot be serialized within authoring.

> [!IMPORTANT] 
> If you have renamed, added, or deleted any ```: IComponent``` in your project, all your serialized EntityBakedDataAsset instances potentially become invalid and will require re-baking, as they are binary. Full rebaking performs automatically after assemblies reload. ~~Re-baking only affects assets that used components types from the modified assemblies.~~ Alternatively, you can disable automatic rebaking and use...

Each time you make a change to your GameObject/Prefab, ```ConvertToEntity``` will invoke all Bake() methods in your authoring components within the full hierarchy and serialize them into the ```EntityBakedData``` asset.

## License

[MIT](https://choosealicense.com/licenses/mit/)