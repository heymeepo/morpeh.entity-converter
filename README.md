# Morpeh Entity Converter

Tools for serializing and converting gameobjects into entities for [Morpeh ECS](https://github.com/scellecs/morpeh).

> [!WARNING]
> This package is in the early stage of development for the Morpeh-2024, which has not yet been released. There will likely be many significant changes.

It is not an alternative to Morpeh providers but is intended for complete decoupling from gameobjects at runtime and primarily exists for use with packages such as
- [morpeh.transforms](https://github.com/heymeepo/morpeh.transforms/tree/stage-2024)
- [morpeh.physics](https://github.com/heymeepo/morpeh.physics/tree/stage-2024)
- [morpeh.graphics](https://github.com/heymeepo/morpeh.graphics)


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

GameObjects are used as configurations for future entities. They are only needed in the editor, so they should have the tag "EditorOnly". ```ConvertToEntity``` sets this tag by default.

**```ConvertToEntity```** is a MonoBehaviour class that you need to attach to each GameObject in the (prefab/scene object) hierarchy that you want to convert into an entity.

**```PrefabBakedDataAsset```**  is a ScriptableObject that is placed in ```ConvertToEntity``` root object. It is a serialized representation of your entity hierarchy based on baked GameObjects, and will be used at runtime.

**```SceneBakedDataAsset```**  is a ScriptableObject similar to ```PrefabBakedDataAsset```, but it is created separately for scenes and does not need to be passed to ```ConvertToEntity```.

**```EcsAuthoring```** is the primary MonoBehaviour component for converting GameObjects into entities. All user-declared authoring components must inherit it.

> [!IMPORTANT] 
> Any **```: EcsAuthoring```** must be wrapped in the ```#if UNITY_EDITOR``` directive.

### Getting started
First, you need to create an global entity converter asset.

To do this, go to **Tools -> Morpeh -> Entity Converter**, and click the ``Create Entity Converter Asset`` button. This will create a main asset in the Plugins folder to store all data and settings for the converter. In this window, you can create assets for baking scenes, adjust settings, and manually manage the baking process.

You can bake both prefabs and entire scenes.
Let's start with a prefab:
- Create a regular empty prefab
- Enter prefab editing mode and add the ``ConvertToEntity`` component
- Create an asset to store the baked data **Create -> ECS -> Baker -> PrefabBakedDataAsset**
- Drag and drop the created PrefabBakedDataAsset into the ``ConvertToEntity`` Baked Data Asset field.
- Save the changes

You have now baked the data from the prefab into your asset. Each time you modify and save the prefab, the data will be automatically re-baked.

At runtime, you will only need the asset itself, and the prefab won't even be included in the build. How to create an entity using this asset is described in the section [Creating entities at runtime](#creating-entities-at-runtime).

At the moment, all you have baked are the transform components, but these is valid data that will create a single entity with ``LocalToWorld`` and ``LocalTransform`` components.

To add more components to the entity, you need to use ``EcsAuthoring`` components. Instructions on how to use them are described below.

Now, let's look at how to bake scenes:
- Open the Entity Converter Window
- Click the ``Create Scene Baked Data Asset`` button next to the scene you want to bake.
- In the folder where your scene is located, an asset will be created to store the baked data from the scene, similar to the ``PrefabBakedDataAsset`` we created earlier for prefab.
- Now you can create GameObject in the scene and add the ConvertToEntity component to it. You can even just drag in the previously created prefab
- Save the changes

Now, each time you save the scene, it will be re-baked into this asset. 

If you dragged the previously created prefab into the scene, its data was baked separately into the scene's asset, without affecting its own ``PrefabBakedDataAsset`` that you assigned to it. It's important to understand that the baked data of prefabs and the baked data of prefab instances placed in scenes do not overlap and are baked separately.

However, it's important to note that if you modify the original prefab using prefab editing mode, all scenes referencing this prefab will be automatically re-baked.

The same applies to nested prefabs and prefab variants. Changing any original dependent object will trigger a re-bake of the entire chain.

### Baking process
The rule is simple: all objects in the prefab or scene hierarchy that have a ```ConvertToEntity``` component attached, will be baked into the entities. All GameObjects in the hierarchy that do not have ```ConvertToEntity``` component attached, will be ignored as if they were never there. 

I strongly recommend against having such objects in the hierarchy, because you may get unexpected results since children entities inherit their local positions relative to the parent, which could be a completely different object from what you expect in the editor.

All transform components such as ```LocalToWorld```, ```LocalTransform```, and ```PostTransformMatrix``` are automatically added during baking, you do not need to add them manually.

``EcsAuthoring`` has two main methods: 
 - ``OnBeforeBake(UserContext userContext)`` 
 - ``OnBake(BakingContext bakingContext, UserContext userContext)``

All ``OnBeforeBake`` methods are called on each authoring component, and only after that, the ``OnBake`` methods are called.

**> Set components**

There are 2 options to add a component to an entity during data baking:

```csharp
public sealed class AwesomeAuthoring : EcsAuthoring
{
    [SerializeField]
    private float someValue;

    public override void OnBake(BakingContext bakingContext, UserContext userContext)
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;

        bakingContext.SetComponent(new AwesomeComponent() 
        { 
            value = someValue,
            mesh = mesh
        });

        bakingContext.SetComponent(new AnotherAwesomeComponent());
    }
}

public struct AwesomeComponent : IComponent
{
    public float value;
    public Mesh mesh;
}
```

```csharp
public sealed class AwesomeAuthoring : EcsAuthoring
{
    [SerializeField]
    private float4 someValue;

    public override void OnBake(BakingContext bakingContext, UserContext userContext)
    {        
        bakingContext.SetComponentUnsafe<float4>(typeof(AwesomeFloat4Component), someValue);
    }
}

public struct AwesomeFloat4Component : IComponent
{
    public float4 value;
}
```

In the second option, you can assign a value to a component using its Type. The ```T data``` in this case can be any structure, either managed or unmanaged, but it must exactly match the memory layout of the component you want to write it into. This is a somewhat dangerous way to add a component to an entity. Use it only if you are absolutely certain that you need to. 

**> EntityLink**

``EntityLink`` is a structure used to pass entities into your components directly from the editor. 

Entities obtained from ``EntityLink`` cannot have components added to them or undergo any standard runtime operations. They are intended only for passing to components. They become valid at runtime after deserialization.

To assign data to an ``EntityLink``, drag and drop any GameObject from the hierarchy that has a ConvertToEntity component into the field.

```csharp
public sealed class VehicleAuthoring : EcsAuthoring
{
    [SerializeField]
    private EntityLink frontLeftWheel;

    [SerializeField]
    private EntityLink frontRightWheel;

    [SerializeField]
    private EntityLink rearLeftWheel;

    [SerializeField]
    private EntityLink rearRightWheel;

    public override void OnBake(BakingContext bakingContext, UserContext userContext)
    {        
        bakingContext.SetComponent(new VehicleComponent()
        {
            frontLeftWheel = bakingContext.GetEntityFromLink(frontLeftWheel),
            frontRightWheel = bakingContext.GetEntityFromLink(frontRightWheel),
            rearLeftWheel = bakingContext.GetEntityFromLink(rearLeftWheel),
            rearRightWheel = bakingContext.GetEntityFromLink(rearRightWheel)
        });
    }
}

public struct VehicleComponent : IComponent
{
    public Entity frontLeftWheel;
    public Entity frontRightWheel;
    public Entity rearLeftWheel;
    public Entity rearRightWheel;
}
```

> [!IMPORTANT] 
> GameObjects that you assign to ``EntityLink`` through the editor must be part of the same hierarchy where you are assigning them. The GameObject must either be part of the same prefab or, in the case of baking scenes, be located in the same scene.

> [!WARNING]
> There is one important limitation. Еntities in your components, you want to assign from editor, cannot be placed inside managed types or arrays. This means you can have such structures in your components, but you cannot pass entities into them using ``BakingContext.GetEntityFromLink()``. In the future, there will be an option to pass them into fixed-size arrays.

```csharp
public struct AwesomeComponent : IComponent
{
    public FixedList128<Entity>                 //Temporary not allowed
    public Entity[] entities;                   //Not allowed
    public SomeManagedType managedType;         //Not allowed
    public SomeUnmanagedType unmanagedType;     //Allowed
    public Entity entity;                       //Allowed
}

public class SomeManagedType
{
    public Entity entity;
}

public struct SomeUnmanagedType
{
    public Entity entity;
}
```

### Creating entities at runtime
So, you have baked your entities into ScriptableObject assets. These can be ``SceneBakedDataAsset`` or ``PrefabBakedDataAsset``. Creating entities with these two assets is essentially the same. Let's look at how to do it using a prefab as an example.

To create entities, we use a factory that can be obtained directly from the asset with the baked data.

```csharp
public sealed class SpawnObstacleSystem : ISystem
{
    private Filter filter;
    private Stash<SpawnObstacleRequest> spawnStash;

    public override void OnAwake()
    {
        filter = World.Filter.With<SpawnObstacleRequest>().Build();
        spawnStash = World.GetStash<SpawnObstacleRequest>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach(var requestEnt in filter)
        {
            ref var request = ref spawnStash.Get(requestEnt);
            var factory = request.config.GetFactory();
            factory.Create(World);
        }
    }
}

public struct SpawnObstacleRequest : IComponent
{
    public PrefabBakedDataAsset config;
}
```

This demonstrates the simplest way to create an entity from the asset config.

The factory also has several other methods for creating entities:
- ``Create(World world, Span<Entity> roots)``
- ``CreateAt(World world, float3 position, quaternion rotation)``
- ``CreateAt(World world, float3 position, quaternion rotation, Span<Entity> roots)``

``CreateAt`` as the name suggests, allows you to create an entity hierarchy at a specific position.

But what is the ``Span<Entity> roots``?

You can pass a span to the factory method that will be filled with only root entities, but not the entire hierarchy. During the GameObjects conversion, your entities may be split into multiple root entities, but will still be baked into the same asset. This often happens when converting a scene, where each scene object with its own hierarchy will be considered as a separate root object. This is described in more detail in the README for the [morpeh.transforms](https://github.com/heymeepo/morpeh.transforms/tree/stage-2024) package. For example, in the case of prefabs, destructible obstacles might be split into several roots.

Typically, when working with entity hierarchies in ECS, you are primarily interested in the root entities. These contain the main components, while their children are secondary. In any case, you can traverse the entire hierarchy down to the lowest level using the root objects if you really need to.

Let's break down how to create our entity hierarchy at the desired position and add additional components to them after creation.

```csharp
public sealed class SpawnObstacleSystem : ISystem
{
    private Filter filter;
    private Stash<SpawnObstacleRequest> spawnStash;
    private Stash<TeamId> teamIdStash;

    public override void OnAwake()
    {
        filter = World.Filter.With<SpawnObstacleRequest>().Build();
        spawnStash = World.GetStash<SpawnObstacleRequest>();
        teamIdStash = World.GetStash<TeamId>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach(var requestEnt in filter)
        {
            ref var request = ref spawnStash.Get(requestEnt);
            var factory = request.config.GetFactory();

            Span<Entity> roots = stackalloc Entity[factory.RootEntitiesCount];
            factory.CreateAt(World, request.position, quaternion.identity, roots);

            for(int i = 0; i < roots.Length; i++)
            {
                var ent = roots[i];
                teamIdStash.Set(ent, new TeamId() { id = request.teamId });
            }
        }
    }
}

public struct SpawnObstacleRequest : IComponent
{
    public PrefabBakedDataAsset config;
    public float3 position;
    public int teamId;
}
```

## License

[MIT](https://choosealicense.com/licenses/mit/)